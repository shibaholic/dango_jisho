import { z } from "zod";
import { TrackedEntrySchema } from "./TrackedEntry";

export const EntryIsTaggedSchema = z.object({
  userOrder: z.number(),
  addedToTagDate: z.string().datetime({ local: true }),
  trackedEntry: z.nullable(TrackedEntrySchema),
});

export type EntryIsTagged = z.infer<typeof EntryIsTaggedSchema>;
