import { Entry } from "@/types/JMDict";
import React from "react";
import ObjectDisplay from "../testing/tracked-entries/objectDisplay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

interface FatListCardProps {
  entry: Entry;
}

const FatListCard = ({ entry }: FatListCardProps) => {
  let faceTermElement = <p>ERROR</p>;

  if (entry.selectedKanjiIndex !== null) {
    faceTermElement = (
      <>
        <p>
          <ruby className="relative inline-block">
            <h3 className="inline-block text-2xl font-bold">
              {entry.kanjiElements[entry.selectedKanjiIndex].keb}
            </h3>

            <rp className="hidden">(</rp>

            <rt className="absolute -top-4 left-0 text-xs">
              {entry.readingElements[entry.selectedReadingIndex].reb}
            </rt>

            <rp className="hidden">)</rp>
          </ruby>
        </p>
      </>
    );
  } else {
    faceTermElement = (
      <>
        <h3 className="inline-block text-2xl font-bold">
          {entry.readingElements[entry.selectedReadingIndex].reb}
        </h3>
      </>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row justify-start">
        <CardTitle>{faceTermElement}</CardTitle>
      </CardHeader>
      <CardContent>
        <div>
          {entry.senses.map((sense, index) => {
            return (
              <div>
                <p>
                  <span className="font-light">{index}. </span>
                  {sense.gloss.map((gloss, index, array) => {
                    let glossSpan = (
                      <span className="font-medium text-lg">{gloss}; </span>
                    );

                    if (index === array.length - 1) {
                      glossSpan = (
                        <span className="font-medium text-lg">{gloss}</span>
                      );
                    }

                    return <>{glossSpan}</>;
                  })}
                  <span className="font-light ml-4">{sense.s_inf} </span>
                </p>
              </div>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
};

export default FatListCard;
