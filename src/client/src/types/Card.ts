import { z } from "zod";
import {
  KanjiElementSchema,
  ReadingElementSchema,
  SenseSchema,
} from "./JMDict";

export const CardSchema = z.object({
  ent_seq: z.string(),
  userId: z.string().nullable(),
  kanjiElement: KanjiElementSchema,
  readingElement: ReadingElementSchema,
  senses: z.array(SenseSchema),
});

export type Card = z.infer<typeof CardSchema>;
