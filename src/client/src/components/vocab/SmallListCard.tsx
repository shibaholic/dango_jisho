import {
  convertTrackedEntryToEntry,
  levelStateColors,
  TrackedEntry,
  TrackedEntrySchema,
} from "@/types/TrackedEntry";
import { FuriganaSegment, annotateFurigana } from "@/utils/furigana";
import React, { useRef } from "react";
import { useNavigate } from "react-router-dom";
import { TableCell, TableRow } from "../ui/table";
import { Menu } from "lucide-react";
import { Button } from "../ui/button";
import TagsModal, { TagsModalHandle } from "./TagsModal";
import { VocabDropdown } from "./VocabDropdown";
import { Entry } from "@/types/JMDict";
import { createPortal } from "react-dom";

interface SmallListCardProps {
  trackedEntry: TrackedEntry;
}

const SmallListCard = ({ trackedEntry }: SmallListCardProps) => {
  const entry = convertTrackedEntryToEntry(trackedEntry);

  const navigate = useNavigate();
  const tagsModalRef = useRef<TagsModalHandle>(null);

  const handleClick = () => {
    const selection = window.getSelection();
    if (selection && selection.toString().length > 0) {
      // If text is selected, don't trigger the onClick event
      return;
    }
    navigate(`/vocab/${entry.ent_seq}`);
  };
  const selectedKanjiIndex = entry.selectedKanjiIndex;

  let faceTermElement = <div>ERROR</div>;

  let kanji = "";
  let reading = "";

  if (selectedKanjiIndex !== null) {
    kanji = entry.kanjiElements[selectedKanjiIndex].keb;
    reading = entry.readingElements[entry.selectedReadingIndex].reb;
    const annotatedSegments: FuriganaSegment[] = annotateFurigana(
      kanji,
      reading
    );

    faceTermElement = (
      <h5
        lang="ja"
        className={`hover:text-[#535bf2] hover:bg-gray-100 cursor-pointer rounded-lg`}
        onMouseUp={handleClick}
      >
        {annotatedSegments.map((segment, index) =>
          segment.furigana ? (
            <ruby key={index} className="mx-0.5 text-2xl font-medium">
              <span>{segment.base}</span>
              <rt className="text-sm font-light">{segment.furigana}</rt>
            </ruby>
          ) : (
            <span key={index} className="text-2xl font-medium">
              {segment.base}
            </span>
          )
        )}
      </h5>
    );
  } else {
    faceTermElement = (
      <h5
        lang="ja"
        className={`hover:text-[#535bf2] hover:bg-gray-100 cursor-pointer rounded-lg text-2xl font-medium place-self-center`}
        onMouseUp={handleClick}
      >
        {entry.readingElements[entry.selectedReadingIndex].reb}
      </h5>
    );
  }

  // get the first 3 glosses, so flatmap every sense -> gloss
  const firstThreeGlosses = entry.senses
    .flatMap((sense) => sense.gloss)
    .slice(0, 3);
  let glossElement = (
    <div className="truncate text-ellipsis whitespace-nowrap oveflow-hidden w-full">
      {firstThreeGlosses.map((gloss, index, array) => {
        let punctuation = "; ";
        if (index === array.length - 1) punctuation = "";
        return (
          <span key={index} className="font-normal text-lg">
            {gloss}
            {punctuation}
          </span>
        );
      })}
    </div>
  );

  return (
    <TableRow className="flex flex-col w-full">
      {/* {flexRender(cell.column.columnDef.cell, cell.getContext())} */}
      <TableCell className="w-full flex flex-col">
        <div className="w-full h-[45.6px] flex flex-row justify-between">
          {faceTermElement}
          <div className="flex flex-col">
            <div className="flex flex-row pr-2">
              {entry.trackedEntry && (
                <div
                  className={`text-${levelStateColors[entry.trackedEntry.levelStateType]} leading-none p-1 font-semibold self-center`}
                >
                  {entry.trackedEntry.levelStateType}
                </div>
              )}
              {tagsModalRef.current && (
                <VocabDropdown openTagsModal={tagsModalRef.current.open} />
              )}
            </div>
          </div>
        </div>

        {glossElement}
        <TagsModal ref={tagsModalRef} entry={entry} />
      </TableCell>
    </TableRow>
  );
};

export default SmallListCard;
