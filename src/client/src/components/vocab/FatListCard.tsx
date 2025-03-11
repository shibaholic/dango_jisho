import { Entry } from "@/types/JMDict";
import React, {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import ObjectDisplay from "../../pages/testing/components/ObjectDisplay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { areStringArraysEqual } from "@/utils/stringUtils";
import { posMapping } from "@/types/enums/Pos";
import { FuriganaSegment, annotateFurigana } from "@/utils/furigana";
import { useNavigate } from "react-router-dom";
import { levelStateColors } from "@/types/TrackedEntry";
import { Button } from "../ui/button";
import { LogOut, Menu, Settings, Tag, UserPen } from "lucide-react";
import { useAuth } from "@/utils/auth";
import { fetchLogout } from "@/utils/api";
import { Separator } from "@radix-ui/react-separator";
import { DropdownRow } from "../dropdown/Dropdown";
import TagsModal, { TagsModalHandle } from "./TagsModal";
import { VocabDropdown } from "./VocabDropdown";

interface FatListCardProps {
  entry: Entry;
  linkToVocab?: boolean | undefined;
}

export const FatListCard = ({
  entry,
  linkToVocab = false,
}: FatListCardProps) => {
  const navigate = useNavigate();
  const { user } = useAuth();

  const [isTagModalReady, setIsTagModalReady] = useState(false);
  const tagsModalRef = useRef<TagsModalHandle>(null);

  useEffect(() => {
    if (tagsModalRef.current) {
      setIsTagModalReady(true);
    }
  }, []);

  const handleClick = () => {
    const selection = window.getSelection();
    if (selection && selection.toString().length > 0) {
      // If text is selected, don't trigger the onClick event
      return;
    }
    linkToVocab && navigate(`/vocab/${entry.ent_seq}`);
  };

  let faceTermElement = <p>ERROR</p>;

  if (entry.selectedKanjiIndex !== null) {
    const kanji = entry.kanjiElements[entry.selectedKanjiIndex].keb;
    const reading = entry.readingElements[entry.selectedReadingIndex].reb;
    const annotatedSegments: FuriganaSegment[] = annotateFurigana(
      kanji,
      reading
    );

    faceTermElement = (
      <h3
        lang="ja"
        className={`${linkToVocab && "hover:text-[#535bf2] hover:bg-gray-100 cursor-pointer"} rounded-lg p-1 relative -top-1 -left-1`}
        onMouseUp={handleClick}
      >
        {annotatedSegments.map((segment, index) =>
          segment.furigana ? (
            <ruby key={index} className="mx-0.5 text-3xl font-medium">
              <span>{segment.base}</span>
              <rt className="text-sm font-light">{segment.furigana}</rt>
            </ruby>
          ) : (
            <span key={index} className="text-3xl font-medium">
              {segment.base}
            </span>
          )
        )}
      </h3>
    );
  } else {
    faceTermElement = (
      <h3
        lang="ja"
        className={`${linkToVocab && "hover:text-[#535bf2] hover:bg-gray-100 cursor-pointer"} rounded-lg p-1 relative -top-1 -left-1 text-3xl font-medium`}
        onMouseUp={handleClick}
      >
        {entry.readingElements[entry.selectedReadingIndex].reb}
      </h3>
    );
  }

  function createPosSpans(pos: string[]): JSX.Element[] {
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
    return poses;
  }

  let glossesSection: JSX.Element[] = [];

  let currentPos = [""];
  entry.senses.forEach((sense, index) => {
    if (!areStringArraysEqual(currentPos, sense.pos)) {
      currentPos = sense.pos;
      glossesSection.push(
        <p key={"pos" + index} className="mt-1">
          {createPosSpans(currentPos)}
        </p>
      );
    }

    glossesSection.push(
      <p
        key={"gloss" + index}
        className="`w-full flex flex-wrap whitespace-pre-wrap"
      >
        <span className="font-light text-xl">{index + 1}. </span>
        {sense.gloss.map((gloss, index, array) => {
          let punctuation = "; ";
          if (index === array.length - 1) punctuation = "";
          return (
            <span key={index} className="font-normal text-xl">
              {gloss}
              {punctuation}
            </span>
          );
        })}

        {sense.s_inf && (
          <span className="font-light text-sm text-zinc-600 self-center">
            {" "}
            {sense.s_inf}
          </span>
        )}
      </p>
    );
  });

  return (
    <div className="relative">
      <Card className="flex flex-col">
        <CardHeader className="flex flex-row justify-between p-6">
          <CardTitle>{faceTermElement}</CardTitle>

          {user && (
            <div className="flex flex-col">
              <div className="flex flex-row">
                {entry.trackedEntry && (
                  <div
                    className={`text-${levelStateColors[entry.trackedEntry.levelStateType]} leading-none p-1 mr-1 font-semibold self-center`}
                  >
                    {entry.trackedEntry.levelStateType}
                  </div>
                )}
                {isTagModalReady && (
                  <VocabDropdown openTagsModal={tagsModalRef.current!.open} />
                )}
              </div>
              <TagsModal ref={tagsModalRef} entry={entry} />
            </div>
          )}
        </CardHeader>
        <CardContent>{glossesSection}</CardContent>
      </Card>
    </div>
  );
};
