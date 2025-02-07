export type Entry = {
  ent_seq: string;
  kanjiElements: KanjiElement[];
  readingElements: ReadingElement[];
  senses: Sense[];
};

export type KanjiElement = {
  ent_seq: string;
  keb: string;
  ke_inf: string[];
  ke_pri: string | null;
};

export type ReadingElement = {
  ent_seq: string;
  reb: string;
  re_nokanji: boolean;
  re_restr: string[];
  re_inf: string[];
  re_pri: string | null;
};

export type Sense = {
  stagk: string[];
  stagr: string[];
  pos: string[];
  xref: string[];
  ant: string[];
  field: string[];
  misc: string[];
  s_inf: string[];
  lsource: LSource[];
  dial: string[];
  gloss: string[];
};

export type LSource = {
  langValue: string;
  lang: string | null;
  ls_part: boolean;
  ls_wasei: boolean;
};
