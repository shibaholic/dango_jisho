import { z } from "zod";

export const LevelState = ["New", "Learning", "Reviewing", "Known"] as const;

export const SpecialCategory = [
  "NeverForget",
  "Blacklist",
  "Cram",
  "Failed",
] as const;

export const TrackedEntrySchema = z.object({
  ent_seq: z.string(),
  userId: z.string(), // Guid
  levelStateType: z.enum(LevelState),
  oldLevelStateType: z.enum(LevelState).nullable(),
  specialCategory: z.enum(SpecialCategory).nullable(),
  score: z.number(),
  lastReviewDate: z.string().datetime({ local: true }).nullable(),
  nextReviewDays: z.number().nullable(),
  nextReviewMinutes: z.number().nullable(),
});

export type TrackedEntry = z.infer<typeof TrackedEntrySchema>;

// export type TrackedEntry = {
//   ent_seq: string;
//   userId: string; // Guid
//   levelStateType: typeof LevelState;
//   oldLevelStateType: typeof LevelState | null;
//   specialCategory: typeof SpecialCategory | null;
//   score: number;
//   lastReviewDate: Date;
//   nextReviewDays: number | null;
//   nextReviewMinutes: number | null;
// };

// export const LevelState = {
//   0: "New",
//   1: "Learning",
//   2: "Reviewing",
//   3: "Known",
// } as const;

// export const SpecialCategory = {
//   0: "NeverForget",
//   1: "Blacklist",
//   2: "Cram",
//   3: "Failed",
// } as const;
