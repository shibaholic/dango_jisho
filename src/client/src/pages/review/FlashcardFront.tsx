import { Button } from "@/components/ui/button";
import { useFlashcardFetch } from "@/hooks/review/useFlashcardFetch";
import { Entry } from "@/types/JMDict";
import { TrackedEntry, convertTrackedEntryToEntry } from "@/types/TrackedEntry";
import React from "react";
import { useNavigate } from "react-router-dom";

interface FlashcardFrontProps {
  ent_seq: string;
  tagId: string;
}

const FlashcardFront = (data: FlashcardFrontProps) => {
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

  let faceTerm = "";

  if (entry.selectedKanjiIndex !== null) {
    // faceTerm is kanji
    faceTerm = entry.kanjiElements[entry.selectedKanjiIndex].keb;
  } else {
    // faceTerm is reading
    faceTerm = entry.readingElements[entry.selectedReadingIndex].reb;
  }

  function onReveal() {
    navigate(`/review/${data.tagId}/${trackedEntry.ent_seq}/back`);
  }

  return (
    <>
      <div></div>
      <div className="flex flex-col relative items-center">
        {trackedEntry.levelStateType === "New" && (
          <div className="absolute top-[-3rem] bg-blue-500 text-white py-1 px-2 text-nowrap rounded-md">
            New
          </div>
        )}
        <h1 lang="ja" className="text-7xl text-nowrap">
          {faceTerm}
        </h1>
      </div>
      <Button onClick={() => onReveal()} size="lg" className="text-2xl">
        Reveal
      </Button>
    </>
  );
};

export default FlashcardFront;
