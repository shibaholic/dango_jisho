import { z } from "zod";
import { TrackedEntrySchema } from "./TrackedEntry";

export const LSourceSchema = z.object({
  langValue: z.string(),
  lang: z.string().nullable(),
  ls_part: z.boolean(),
  ls_wasei: z.boolean(),
});

export const SenseSchema = z.object({
  stagk: z.array(z.string()),
  stagr: z.array(z.string()),
  pos: z.array(z.string()),
  xref: z.array(z.string()),
  ant: z.array(z.string()),
  field: z.array(z.string()),
  misc: z.array(z.string()),
  s_inf: z.array(z.string()),
  lsource: z.array(LSourceSchema),
  dial: z.array(z.string()),
  gloss: z.array(z.string()),
});

export const KanjiElementSchema = z.object({
  // ent_seq: z.string(),
  keb: z.string(),
  ke_inf: z.array(z.string()),
  ke_pri: z.string().nullable(),
});

export const ReadingElementSchema = z.object({
  // ent_seq: z.string(),
  reb: z.string(),
  re_nokanji: z.boolean(),
  re_restr: z.array(z.string()),
  re_inf: z.array(z.string()),
  re_pri: z.string().nullable(),
});

export const EntrySchema = z.lazy(() =>
  z.object({
    ent_seq: z.string(),
    selectedKanjiIndex: z.number().nullable(),
    selectedReadingIndex: z.number(),
    priorityScore: z.number().nullable(),
    kanjiElements: z.array(KanjiElementSchema),
    readingElements: z.array(ReadingElementSchema),
    senses: z.array(SenseSchema),
    trackedEntry: TrackedEntrySchema.optional(),
  })
);

export type Entry = z.infer<typeof EntrySchema>;
export type KanjiElement = z.infer<typeof KanjiElementSchema>;
export type ReadingElement = z.infer<typeof ReadingElementSchema>;
export type Sense = z.infer<typeof SenseSchema>;
export type LSource = z.infer<typeof LSourceSchema>;

// export type Entry = {
//   ent_seq: string;
//   kanjiElements: KanjiElement[];
//   readingElements: ReadingElement[];
//   senses: Sense[];
// };

// export type KanjiElement = {
//   ent_seq: string;
//   keb: string;
//   ke_inf: string[];
//   ke_pri: string | null;
// };

// export type ReadingElement = {
//   ent_seq: string;
//   reb: string;
//   re_nokanji: boolean;
//   re_restr: string[];
//   re_inf: string[];
//   re_pri: string | null;
// };

// export type Sense = {
//   stagk: string[];
//   stagr: string[];
//   pos: string[];
//   xref: string[];
//   ant: string[];
//   field: string[];
//   misc: string[];
//   s_inf: string[];
//   lsource: LSource[];
//   dial: string[];
//   gloss: string[];
// };

// export type LSource = {
//   langValue: string;
//   lang: string | null;
//   ls_part: boolean;
//   ls_wasei: boolean;
// };
