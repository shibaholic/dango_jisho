"use client";

import { TrackedEntry } from "@/types/TrackedEntry";
import { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<TrackedEntry>[] = [
  {
    header: "ent_seq",
    accessorKey: "ent_seq",
  },
  {
    header: "Level State",
    accessorKey: "levelStateType",
  },
  {
    header: "Special Category",
    accessorKey: "specialCategory",
  },
  {
    header: "Score",
    accessorKey: "score",
  },
  {
    header: "Last Review Date",
    accessorKey: "lastReviewDate",
  },
  {
    header: "Next Review Days",
    accessorKey: "nextReviewDays",
  },
  {
    header: "Next Review Minutes",
    accessorKey: "nextReviewMinutes",
  },
];
