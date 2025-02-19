// utils/furigana.ts
export interface FuriganaSegment {
  base: string;
  furigana?: string;
}

const isKanji = (char: string) => /[\u4e00-\u9faf]/.test(char);

export function annotateFurigana(
  written: string,
  reading: string
): FuriganaSegment[] {
  if (!written.length) return [];

  // Split the written text into segments.
  const segments: { text: string; type: "kanji" | "kana" }[] = [];
  let currentSegment = written[0];
  let currentType: "kanji" | "kana" = isKanji(written[0]) ? "kanji" : "kana";

  for (let i = 1; i < written.length; i++) {
    const char = written[i];
    const charType = isKanji(char) ? "kanji" : "kana";
    if (charType === currentType) {
      currentSegment += char;
    } else {
      segments.push({ text: currentSegment, type: currentType });
      currentSegment = char;
      currentType = charType;
    }
  }
  segments.push({ text: currentSegment, type: currentType });

  // Now “align” the reading.
  // The assumption is that the reading string is the furigana for all kanji
  // plus the kana segments (which we render without ruby)
  let annotated: FuriganaSegment[] = [];
  let readingPointer = 0;

  // console.log("segments");
  // console.log(segments);

  for (let i = 0; i < segments.length; i++) {
    const segment = segments[i];
    if (segment.type === "kana") {
      // This part is okurigana – it should appear verbatim.
      annotated.push({ base: segment.text });
      readingPointer += segment.text.length;
    } else {
      // For a kanji segment, look ahead for the next kana segment.
      const nextKana = segments.slice(i + 1).find((s) => s.type === "kana");

      // console.log("segments.slice(i + 1): ");
      // console.log(segments.slice(i + 1));
      // console.log("segments[i]: ");
      // console.log(segments[i]);

      let furigana = "";
      if (nextKana) {
        // Find where the next kana segment begins in the reading string.

        // always check one after readingPointer, since then we make sure to skip at least one kana that is (i think) guaranteed to be the current kanji's
        let nextIndex = reading.indexOf(nextKana.text, readingPointer + 1);

        // console.log("nextIndex: " + nextIndex);

        if (nextIndex !== -1) {
          // if the nextKana segment WAS found in the reading
          furigana = reading.substring(readingPointer, nextIndex);
          // console.log("furigana nextKana in reading: " + furigana);
          readingPointer = nextIndex;
        } else {
          // if the nextKana segment was not found in the reading
          furigana = reading.substring(readingPointer);
          // console.log("furigana nextKana NOT in reading: " + furigana);
          readingPointer = reading.length;
        }
      } else {
        // If no kana follows, use the rest of the reading.
        furigana = reading.substring(readingPointer);
        readingPointer = reading.length;
      }
      annotated.push({ base: segment.text, furigana });
    }
  }

  return annotated;
}
