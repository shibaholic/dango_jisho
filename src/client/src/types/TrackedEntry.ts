export type TrackedEntry = {
  ent_seq: string;
  userId: string; // Guid
  levelStateType: LevelState;
  oldLevelStateType: LevelState | null;
  specialCategory: SpecialCategory | null;
  score: number;
  lastReviewDate: Date;
  nextReviewDays: number | null;
  nextReviewMinutes: number | null;
};

export const LevelStateMap = {
  0: "New",
  1: "Learning",
  2: "Reviewing",
  3: "Known",
} as const;

export type LevelState = keyof typeof LevelStateMap;

export function levelStateToString(levelState: LevelState | null): string {
  if (levelState) return LevelStateMap[levelState] || "Unknown LevelState";
  return "null";
}

export const SpecialCategoryMap = {
  0: "NeverForget",
  1: "Blacklist",
  2: "Cram",
  3: "Failed",
} as const;

export type SpecialCategory = keyof typeof SpecialCategoryMap;

export function specialCategoryToString(
  specialCategory: SpecialCategory | null
): string {
  if (specialCategory)
    return SpecialCategoryMap[specialCategory] || "Unknown SpecialCategory";
  return "null";
}
