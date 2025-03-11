import { z } from "zod";
import { EntryIsTaggedSchema } from "./EntryIsTagged";

// export const Tag_EITSchema = z.object({
//   id: z.string(),
//   name: z.string(),
//   created: z.string().datetime({ local: true }),
//   totalEntries: z.number(),
//   totalNew: z.number(),
//   totalLearning: z.number(),
//   totalReviewing: z.number(),
//   totalKnown: z.number(),
//   entryIsTaggeds: z.array(z.lazy(() => EIT_TESchema)),
// });

export const TagSchema = z.lazy(() =>
  z.object({
    id: z.string(),
    name: z.string(),
    created: z.string().datetime({ local: true }),
    totalEntries: z.number(),
    totalNew: z.number(),
    totalLearning: z.number(),
    totalReviewing: z.number(),
    totalKnown: z.number(),
    entryIsTaggeds: z.array(EntryIsTaggedSchema).default([]),
  })
);

// export type Tag_EIT = z.infer<typeof Tag_EITSchema>;
export type Tag = z.infer<typeof TagSchema>;
