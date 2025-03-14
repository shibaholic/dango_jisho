import { TrackedEntry } from "@/types/TrackedEntry";
import { fetchEntry, fetchNextReview, fetchTrackedEntry } from "@/utils/api";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";

export const useNextFlashcardFetch = (tagId: string) => {
  const { data, isError, error, isLoading } = useQuery({
    queryKey: ["next-review"],
    staleTime: 0,
    queryFn: async () => await fetchNextReview(tagId),
  });

  return { data, isError, error, isLoading };
};

export const getNextFrontUrl = async (tagId: string) => {
  try {
    const nextFlashcard = await fetchNextReview(tagId);
    if (nextFlashcard.data) {
      console.log("navigate to front");
      return `/review/${tagId}/${nextFlashcard.data.ent_seq}/front`;
    } else {
      return `finished`;
    }
  } catch (error) {
    console.error(`useGetNextFront error: ${error}`);
  }
  return undefined;
};

export const useFlashcardFetch = (ent_seq: string) => {
  const { data, isError, error, isLoading } = useQuery({
    queryKey: ["entry", ent_seq],
    staleTime: 0,
    queryFn: async () => await fetchTrackedEntry(ent_seq),
  });

  return { data, isError, error, isLoading };
};
