"use client";

import { ColumnDef } from "@tanstack/react-table";

// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
// export type Payment = {
//   id: string;
//   amount: number;
//   status: "pending" | "processing" | "success" | "failed";
//   email: string;
// };

// export const columns: ColumnDef<Payment>[] = [
//   {
//     accessorKey: "status",
//     header: "Status",
//   },
//   {
//     accessorKey: "email",
//     header: "Email",
//   },
//   {
//     accessorKey: "amount",
//     header: "Amount",
//   },
// ];

export type TrackedEntry = {
  ent_seq: string;
  userId: string; // Guid
  levelStateType: LevelStateType;
  oldLevelStateType: LevelStateType | null;
  specialCategory: SpecialCategory | null;
  score: number;
  lastReviewDate: Date;
  nextReviewDays: number | null;
  nextReviewMinutes: number | null;
};

export enum LevelStateType {
  New,
  Learning,
  Reviewing,
  Known,
}

export enum SpecialCategory {
  NeverForget,
  Blacklist,
  Cram,
  Failed,
}

export const columns: ColumnDef<TrackedEntry>[] = [
  {
    header: "ent_seq",
    accessorKey: "ent_seq",
  },
  {
    header: "Level State",
    accessorKey: "levelStateType",
  },
  {
    header: "Special Category",
    accessorKey: "specialCategory",
  },
  {
    header: "Score",
    accessorKey: "score",
  },
  {
    header: "Last Review Date",
    accessorKey: "lastReviewDate",
  },
  {
    header: "Next Review Days",
    accessorKey: "nextReviewDays",
  },
  {
    header: "Next Review Minutes",
    accessorKey: "nextReviewMinutes",
  },
];
