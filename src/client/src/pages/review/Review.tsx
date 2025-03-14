import { Button } from "@/components/ui/button";
import {
  useFlashcardFetch,
  getNextFrontUrl,
  useNextFlashcardFetch,
} from "@/hooks/review/useFlashcardFetch";
import { Entry } from "@/types/JMDict";
import { TrackedEntry, TrackedEntrySchema } from "@/types/TrackedEntry";
import {
  commandChangeEvent,
  commandReviewEvent,
  fetchNextReview,
} from "@/utils/api";
import { useMutation, useQuery } from "@tanstack/react-query";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet-async";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { z } from "zod";
import FlashcardFront from "./FlashcardFront";
import { useAuth } from "@/utils/auth";
import FlashcardBack from "./FlashcardBack";

const Review = () => {
  const { tagId, ent_seq, side, finished } = useParams();
  const navigate = useNavigate();

  const [finishedState, setFinishedState] = useState<boolean>(false);

  useEffect(() => {
    async function asyncEffect() {
      if (!ent_seq) {
        const nextUrl = await getNextFrontUrl(tagId ?? "");
        if (nextUrl === "finished") {
          setFinishedState(true);
        } else if (nextUrl) {
          navigate(nextUrl);
        } else {
          console.error("useGetNextFront failed.");
        }
      }
    }
    asyncEffect();
  }, [tagId, navigate]);

  let contents = <p>default</p>;

  if (finished || finishedState) {
    contents = <p>No more cards due.</p>;
  } else if (tagId && ent_seq) {
    console.log();
    if (side === "front") {
      contents = <FlashcardFront tagId={tagId} ent_seq={ent_seq} />;
    } else if (side === "back") {
      contents = <FlashcardBack tagId={tagId} ent_seq={ent_seq} />;
    }
  }

  return (
    <>
      <Helmet>
        <title> Review - Dango Jisho </title>
      </Helmet>
      <div className="w-full h-full flex flex-col items-center">
        <div className="h-full flex flex-col justify-between items-center p-4">
          {contents}
        </div>
      </div>
    </>
  );
};

export default Review;
