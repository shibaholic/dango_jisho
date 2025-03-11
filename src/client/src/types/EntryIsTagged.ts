import { z } from "zod";
import { TrackedEntrySchema } from "./TrackedEntry";
import { TagSchema } from "./Tag";

// export const EIT_TESchema = z.object({
//   userOrder: z.number(),
//   addedToTagDate: z.string().datetime({ local: true }),
//   trackedEntry: z.nullable(z.lazy(() => TrackedEntrySchema)),
// });

export const EntryIsTaggedSchema = z.lazy(() =>
  z.object({
    userOrder: z.number(),
    addedToTagDate: z.string().datetime({ local: true }),
    tag: TagSchema.optional(),
    trackedEntry: TrackedEntrySchema.optional(),
  })
);

// export type EIT_TE = z.infer<typeof EIT_TESchema>;
export type EntryIsTagged = z.infer<typeof EntryIsTaggedSchema>;
