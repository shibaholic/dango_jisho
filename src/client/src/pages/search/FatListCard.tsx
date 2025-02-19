import { Entry } from "@/types/JMDict";
import React from "react";
import ObjectDisplay from "../testing/tracked-entries/objectDisplay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { areStringArraysEqual } from "@/utils/stringUtils";
import { posMapping } from "@/types/enums/Pos";
import { FuriganaSegment, annotateFurigana } from "@/utils/furigana";

interface FatListCardProps {
  entry: Entry;
}

const FatListCard = ({ entry }: FatListCardProps) => {
  let faceTermElement = <p>ERROR</p>;

  if (entry.selectedKanjiIndex !== null) {
    const kanji = entry.kanjiElements[entry.selectedKanjiIndex].keb;
    const reading = entry.readingElements[entry.selectedReadingIndex].reb;
    const annotatedSegments: FuriganaSegment[] = annotateFurigana(
      kanji,
      reading
    );

    faceTermElement = (
      <>
        <h3>
          {annotatedSegments.map((segment, index) =>
            segment.furigana ? (
              <ruby key={index} className="mx-0.5 text-3xl font-medium">
                {segment.base}
                <rt className="text-sm font-light">{segment.furigana}</rt>
              </ruby>
            ) : (
              <span key={index} className="text-3xl font-medium">
                {segment.base}
              </span>
            )
          )}
        </h3>
      </>
    );
  } else {
    faceTermElement = (
      <>
        <h3 className="text-3xl font-medium">
          {entry.readingElements[entry.selectedReadingIndex].reb}
        </h3>
      </>
    );
  }

  function createPosSpans(pos: string[]): JSX.Element {
    const poses = currentPos.map((pos_str, index) => {
      let punctuation = ", ";
      if (index === pos.length - 1) punctuation = "";
      return (
        <span key={index} className="font-medium text-zinc-700">
          {posMapping[pos_str]}
          {punctuation}
        </span>
      );
    });
    return <p className="mt-1">{poses}</p>;
  }

  let glossesSection: JSX.Element[] = [];

  let currentPos = [""];
  entry.senses.forEach((sense, index) => {
    if (!areStringArraysEqual(currentPos, sense.pos)) {
      currentPos = sense.pos;
      glossesSection.push(
        <div key={"pos" + index}>{createPosSpans(currentPos)}</div>
      );
    }

    glossesSection.push(
      <div key={"gloss" + index} className="flex lg:flex-row flex-col">
        <p className="flex-shrink-0">
          <span className="font-light text-xl">{index + 1}. </span>

          {sense.gloss.map((gloss, index, array) => {
            let punctuation = ", ";
            if (index === array.length - 1) punctuation = "";
            return (
              <span key={index} className="font-medium text-xl">
                {gloss}
                {punctuation}
              </span>
            );
          })}
        </p>
        <p className="flex-shrink-1 ml-4">
          <span className="font-light text-sm text-zinc-600">
            {sense.s_inf}{" "}
          </span>
        </p>
      </div>
    );
  });

  return (
    <Card>
      <CardHeader className="flex flex-row justify-start mt-2">
        <CardTitle>{faceTermElement}</CardTitle>
      </CardHeader>
      <CardContent>
        <div>{glossesSection}</div>
      </CardContent>
    </Card>
  );
};

export default FatListCard;
