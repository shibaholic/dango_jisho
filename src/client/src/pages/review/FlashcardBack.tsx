import { TrackedEntry, convertTrackedEntryToEntry } from "@/types/TrackedEntry";
import { commandReviewEvent, commandChangeEvent } from "@/utils/api";
import { useMutation } from "@tanstack/react-query";
import React from "react";
import { Button } from "../../components/ui/button";
import { FatListCard } from "@/components/vocab/FatListCard";
import { useNavigate } from "react-router-dom";
import {
  getNextFrontUrl,
  useFlashcardFetch,
} from "@/hooks/review/useFlashcardFetch";

interface FlashcardBackProps {
  tagId: string;
  ent_seq: string;
}

const FlashcardBack = (data: FlashcardBackProps) => {
  const navigate = useNavigate();

  const flashcard = useFlashcardFetch(data.ent_seq);
  if (flashcard.isLoading) return <p>Loading...</p>;
  if (flashcard.isError) {
    return <p>Error: {flashcard.error?.message}</p>;
  }
  const trackedEntry = flashcard.data?.data ?? undefined;
  const entry = trackedEntry
    ? convertTrackedEntryToEntry(trackedEntry)
    : undefined;

  const reviewMutation = useMutation({
    mutationKey: ["review-value"],
    mutationFn: async (value: string) =>
      await commandReviewEvent({
        ent_seq: trackedEntry.ent_seq,
        value: value,
      }),
  });

  function submitReview(value: string) {
    reviewMutation
      .mutateAsync(value)
      .then((data) => {
        goToNext();
      })
      .catch((error) => {
        console.error(error);
      });
  }

  const changeMutation = useMutation({
    mutationKey: ["change-value"],
    mutationFn: async (value: string) =>
      await commandChangeEvent({
        ent_seq: trackedEntry.ent_seq,
        value: value,
      }),
  });

  async function submitChange(value: string) {
    console.log("submitChange");
    changeMutation
      .mutateAsync(value)
      .then((data) => {
        goToNext();
      })
      .catch((error) => {
        console.error(error);
      });
  }

  async function goToNext() {
    const nextUrl = await getNextFrontUrl(data.tagId);
    if (nextUrl === "finished") {
      navigate(`/review/${data.tagId}/finished`);
    } else if (nextUrl) {
      navigate(nextUrl);
    } else {
      console.error("useGetNextFront failed.");
    }
  }

  return (
    <div className="flex flex-col h-full justify-between items-center">
      <div></div>
      <FatListCard entry={entry} linkToVocab={false} />
      <div className="flex-end">
        {trackedEntry.levelStateType === "New" ? (
          <>
            <Button className="bg-blue-500" onClick={() => submitChange("New")}>
              New
            </Button>
          </>
        ) : trackedEntry.levelStateType === "Learning" ||
          trackedEntry.levelStateType === "Reviewing" ? (
          <>
            <Button
              className="bg-red-500"
              onClick={() => submitReview("Again")}
            >
              Again
            </Button>
            <Button
              className="bg-green-500"
              onClick={() => submitReview("Okay")}
            >
              Okay
            </Button>
          </>
        ) : (
          trackedEntry.levelStateType === "Known" && (
            <>
              <Button
                className="bg-red-500"
                onClick={() => submitReview("Again")}
              >
                Again
              </Button>
              <Button
                className="bg-yellow-600"
                onClick={() => submitReview("Soon")}
              >
                Soon
              </Button>
              <Button
                className="bg-green-500"
                onClick={() => submitReview("Okay")}
              >
                Okay
              </Button>
              <Button
                className="bg-blue-500"
                onClick={() => submitReview("Easy")}
              >
                Easy
              </Button>
            </>
          )
        )}
      </div>
    </div>
  );
};

export default FlashcardBack;
