import { z } from "zod";
import { EntryIsTaggedSchema } from "./EntryIsTagged";

export const TagSchema = z.object({
  id: z.string(),
  name: z.string(),
  created: z.string().datetime({ local: true }),
  totalEntries: z.number(),
  totalNew: z.number(),
  totalLearning: z.number(),
  totalReviewing: z.number(),
  totalKnown: z.number(),
  entryIsTaggeds: z.array(EntryIsTaggedSchema),
});

export type Tag = z.infer<typeof TagSchema>;
