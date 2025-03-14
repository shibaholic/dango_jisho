import { z } from "zod";
import { Entry, EntrySchema } from "./JMDict";
import { EntryIsTaggedSchema } from "./EntryIsTagged";

export const LevelState = ["New", "Learning", "Reviewing", "Known"] as const;
export type LevelStateType = (typeof LevelState)[number];

export const SpecialCategory = [
  "NeverForget",
  "Blacklist",
  "Cram",
  "Failed",
] as const;

export const TrackedEntrySchema = z.lazy(() =>
  z.object({
    ent_seq: z.string(),
    userId: z.string(), // Guid
    levelStateType: z.enum(LevelState),
    oldLevelStateType: z.enum(LevelState).nullable(),
    specialCategory: z.enum(SpecialCategory).nullable(),
    score: z.number(),
    lastReviewDate: z.string().datetime({ local: true }).nullable(),
    spacedTime: z.string().nullable(),
    entry: EntrySchema.optional(),
    entryIsTaggeds: z.array(EntryIsTaggedSchema).default([]),
  })
);

export type TrackedEntry = z.infer<typeof TrackedEntrySchema>;

export const levelStateColors: Record<LevelStateType, string> = {
  New: "blue-500",
  Learning: " text-[#D97706]",
  Reviewing: " text-[#FCD34D]",
  Known: "green-500",
};

export function convertTrackedEntryToEntry(trackedEntry: TrackedEntry): Entry {
  if (trackedEntry === null)
    throw new Error("null value in convertTrackedEntryToEntry.");
  const entry: Entry = {
    ent_seq: trackedEntry.ent_seq,
    selectedKanjiIndex: trackedEntry.entry.selectedKanjiIndex,
    selectedReadingIndex: trackedEntry.entry.selectedReadingIndex,
    priorityScore: trackedEntry.entry.priorityScore,
    kanjiElements: trackedEntry.entry.kanjiElements,
    readingElements: trackedEntry.entry.readingElements,
    senses: trackedEntry.entry.senses,
    trackedEntry: trackedEntry,
  };

  return entry;
}
