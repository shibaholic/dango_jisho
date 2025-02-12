import { Entry } from "@/types/JMDict";
import React from "react";
import ObjectDisplay from "../testing/tracked-entries/objectDisplay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { areStringArraysEqual } from "@/utils/stringUtils";
import { posMapping } from "@/types/enums/Pos";

interface FatListCardProps {
  entry: Entry;
}

const FatListCard = ({ entry }: FatListCardProps) => {
  let faceTermElement = <p>ERROR</p>;

  if (entry.selectedKanjiIndex !== null) {
    faceTermElement = (
      <>
        <ruby className="relative inline-block">
          <h3 className="inline-block text-3xl font-bold">
            {entry.kanjiElements[entry.selectedKanjiIndex].keb}
          </h3>

          <rp className="hidden">(</rp>

          <rt className="absolute -top-6 left-1.5 text-lg">
            {entry.readingElements[entry.selectedReadingIndex].reb}
          </rt>

          <rp className="hidden">)</rp>
        </ruby>
      </>
    );
  } else {
    faceTermElement = (
      <>
        <h3 className="inline-block text-3xl font-bold">
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
        <span className="font-light text-gray-800">
          {posMapping[pos_str]}
          {punctuation}
        </span>
      );
    });
    return <p>{poses}</p>;
  }

  let glossesSection: JSX.Element[] = [];

  let currentPos = [""];

  entry.senses.forEach((sense, index) => {
    if (!areStringArraysEqual(currentPos, sense.pos)) {
      currentPos = sense.pos;
      glossesSection.push(<>{createPosSpans(currentPos)}</>);
    }

    glossesSection.push(
      <p className="leading-loose">
        <span className="font-light text-xl">{index}. </span>

        {sense.gloss.map((gloss, index, array) => {
          let punctuation = ", ";
          if (index === array.length - 1) punctuation = "";
          return (
            <span className="font-medium text-xl">
              {gloss}
              {punctuation}
            </span>
          );
        })}

        <span className="font-light ml-4">{sense.s_inf} </span>
      </p>
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
