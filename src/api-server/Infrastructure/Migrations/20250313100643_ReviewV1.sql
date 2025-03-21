﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Entries" (
    ent_seq text NOT NULL,
    "SelectedKanjiIndex" integer,
    "SelectedReadingIndex" integer NOT NULL,
    "PriorityScore" integer,
    CONSTRAINT "PK_Entries" PRIMARY KEY (ent_seq)
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Username" text NOT NULL,
    "Password" text NOT NULL,
    "Email" text,
    "RefreshToken" uuid,
    "IsAdmin" boolean NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "KanjiElements" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    ent_seq text NOT NULL,
    keb text NOT NULL,
    ke_inf text[] NOT NULL,
    ke_pri text,
    CONSTRAINT "PK_KanjiElements" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_KanjiElements_Entries_ent_seq" FOREIGN KEY (ent_seq) REFERENCES "Entries" (ent_seq) ON DELETE CASCADE
);

CREATE TABLE "ReadingElements" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    ent_seq text NOT NULL,
    reb text NOT NULL,
    re_nokanji boolean NOT NULL,
    re_restr text[] NOT NULL,
    re_inf text[] NOT NULL,
    re_pri text,
    CONSTRAINT "PK_ReadingElements" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ReadingElements_Entries_ent_seq" FOREIGN KEY (ent_seq) REFERENCES "Entries" (ent_seq) ON DELETE CASCADE
);

CREATE TABLE "Senses" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    ent_seq text NOT NULL,
    stagk text[] NOT NULL,
    stagr text[] NOT NULL,
    pos text[] NOT NULL,
    xref text[] NOT NULL,
    ant text[] NOT NULL,
    field text[] NOT NULL,
    misc text[] NOT NULL,
    s_inf text[] NOT NULL,
    dial text[] NOT NULL,
    gloss text[] NOT NULL,
    CONSTRAINT "PK_Senses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Senses_Entries_ent_seq" FOREIGN KEY (ent_seq) REFERENCES "Entries" (ent_seq) ON DELETE CASCADE
);

CREATE TABLE "StudySets" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "LastStartDate" timestamp with time zone,
    "NewEntryGoal" integer NOT NULL,
    "NewEntryCount" integer NOT NULL,
    "NewQueue" text[] NOT NULL,
    "LearningQueue" text[] NOT NULL,
    "BaseQueue" text[] NOT NULL,
    CONSTRAINT "PK_StudySets" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StudySets_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Tags" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Created" timestamp NOT NULL DEFAULT (NOW()),
    "TotalEntries" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tags_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "TrackedEntries" (
    ent_seq text NOT NULL,
    "UserId" uuid NOT NULL,
    "LevelStateType" text NOT NULL,
    "OldLevelStateType" text,
    "SpecialCategory" text,
    "Score" integer NOT NULL DEFAULT 0,
    "LastReviewDate" timestamp,
    "NextReviewDays" integer,
    "NextReviewMinutes" integer,
    CONSTRAINT "PK_TrackedEntries" PRIMARY KEY (ent_seq, "UserId"),
    CONSTRAINT "FK_TrackedEntries_Entries_ent_seq" FOREIGN KEY (ent_seq) REFERENCES "Entries" (ent_seq) ON DELETE CASCADE,
    CONSTRAINT "FK_TrackedEntries_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Cards" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    ent_seq text NOT NULL,
    "UserId" uuid,
    "KanjiId" integer,
    "ReadingId" integer NOT NULL,
    CONSTRAINT "PK_Cards" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Cards_KanjiElements_KanjiId" FOREIGN KEY ("KanjiId") REFERENCES "KanjiElements" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Cards_ReadingElements_ReadingId" FOREIGN KEY ("ReadingId") REFERENCES "ReadingElements" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "LSource" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "SenseId" integer NOT NULL,
    "LangValue" text NOT NULL,
    lang text,
    ls_part boolean NOT NULL,
    ls_wasei boolean NOT NULL,
    CONSTRAINT "PK_LSource" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LSource_Senses_SenseId" FOREIGN KEY ("SenseId") REFERENCES "Senses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TagInStudySets" (
    "TagId" uuid NOT NULL,
    "StudySetId" uuid NOT NULL,
    "Order" integer NOT NULL,
    CONSTRAINT "PK_TagInStudySets" PRIMARY KEY ("TagId", "StudySetId"),
    CONSTRAINT "FK_TagInStudySets_StudySets_StudySetId" FOREIGN KEY ("StudySetId") REFERENCES "StudySets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TagInStudySets_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "EntryEvents" (
    ent_seq text NOT NULL,
    "UserId" uuid NOT NULL,
    "Serial" integer GENERATED BY DEFAULT AS IDENTITY,
    "Created" timestamp NOT NULL DEFAULT (NOW()),
    "EventType" integer NOT NULL,
    "ReviewValue" text,
    "ChangeValue" text,
    CONSTRAINT "PK_EntryEvents" PRIMARY KEY (ent_seq, "UserId", "Serial"),
    CONSTRAINT "FK_EntryEvents_TrackedEntries_ent_seq_UserId" FOREIGN KEY (ent_seq, "UserId") REFERENCES "TrackedEntries" (ent_seq, "UserId") ON DELETE CASCADE
);

CREATE TABLE "EntryIsTagged" (
    ent_seq text NOT NULL,
    "UserId" uuid NOT NULL,
    "TagId" uuid NOT NULL,
    "AddedToTagDate" timestamp NOT NULL DEFAULT (NOW()),
    "UserOrder" integer NOT NULL,
    CONSTRAINT "PK_EntryIsTagged" PRIMARY KEY (ent_seq, "UserId", "TagId"),
    CONSTRAINT "FK_EntryIsTagged_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EntryIsTagged_TrackedEntries_ent_seq_UserId" FOREIGN KEY (ent_seq, "UserId") REFERENCES "TrackedEntries" (ent_seq, "UserId") ON DELETE CASCADE
);

CREATE TABLE "CardSenses" (
    "CardId" integer NOT NULL,
    "SenseId" integer NOT NULL,
    CONSTRAINT "PK_CardSenses" PRIMARY KEY ("CardId", "SenseId"),
    CONSTRAINT "FK_CardSenses_Cards_CardId" FOREIGN KEY ("CardId") REFERENCES "Cards" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CardSenses_Senses_SenseId" FOREIGN KEY ("SenseId") REFERENCES "Senses" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Cards_KanjiId" ON "Cards" ("KanjiId");

CREATE INDEX "IX_Cards_ReadingId" ON "Cards" ("ReadingId");

CREATE INDEX "IX_CardSenses_SenseId" ON "CardSenses" ("SenseId");

CREATE INDEX "IX_EntryIsTagged_TagId" ON "EntryIsTagged" ("TagId");

CREATE INDEX "IX_KanjiElements_ent_seq" ON "KanjiElements" (ent_seq);

CREATE INDEX "IX_LSource_SenseId" ON "LSource" ("SenseId");

CREATE INDEX "IX_ReadingElements_ent_seq" ON "ReadingElements" (ent_seq);

CREATE INDEX "IX_Senses_ent_seq" ON "Senses" (ent_seq);

CREATE INDEX "IX_StudySets_UserId" ON "StudySets" ("UserId");

CREATE INDEX "IX_TagInStudySets_StudySetId" ON "TagInStudySets" ("StudySetId");

CREATE INDEX "IX_Tags_UserId" ON "Tags" ("UserId");

CREATE INDEX "IX_TrackedEntries_UserId" ON "TrackedEntries" ("UserId");

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250227111613_InitialCreate', '8.0.13');

COMMIT;

START TRANSACTION;

CREATE INDEX "IX_Sense_Gloss" ON "Senses" (gloss);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250227124455_GlossIndex', '8.0.13');

COMMIT;

START TRANSACTION;

DROP TABLE "CardSenses";

DROP TABLE "Cards";

ALTER TABLE "TrackedEntries" DROP COLUMN "NextReviewDays";

ALTER TABLE "TrackedEntries" DROP COLUMN "NextReviewMinutes";

ALTER TABLE "Tags" DROP COLUMN "Created";

ALTER TABLE "TrackedEntries" ALTER COLUMN "LastReviewDate" TYPE timestamp with time zone;

ALTER TABLE "TrackedEntries" ADD "SpacedTime" interval;

ALTER TABLE "Tags" ADD "DateCreated" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Tags" ADD "TotalKnown" integer NOT NULL DEFAULT 0;

ALTER TABLE "Tags" ADD "TotalLearning" integer NOT NULL DEFAULT 0;

ALTER TABLE "Tags" ADD "TotalNew" integer NOT NULL DEFAULT 0;

ALTER TABLE "Tags" ADD "TotalReviewing" integer NOT NULL DEFAULT 0;

ALTER TABLE "EntryIsTagged" ALTER COLUMN "AddedToTagDate" TYPE timestamp with time zone;
ALTER TABLE "EntryIsTagged" ALTER COLUMN "AddedToTagDate" DROP DEFAULT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250313090520_ReviewV1', '8.0.13');

COMMIT;

