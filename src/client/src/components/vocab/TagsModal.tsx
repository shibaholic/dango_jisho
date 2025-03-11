import React, {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useState,
} from "react";
import { Card, CardHeader, CardTitle, CardContent } from "../ui/card";
import { useUserData } from "@/utils/userDataProvider";
import { Checkbox } from "../ui/checkbox";
import { useMutation } from "@tanstack/react-query";
import { CommandEntryTagsProps, commandEntryTags } from "@/utils/api";
import { debounce } from "@/utils/debounce";
import { useAuth } from "@/utils/auth";
import { EntryIsTagged } from "@/types/EntryIsTagged";
import { Entry } from "@/types/JMDict";

interface TagsModalProps {
  entry: Entry;
}

export interface TagsModalHandle {
  close: () => void;
  open: () => void;
}

const TagsModal = forwardRef<TagsModalHandle, TagsModalProps>((props, ref) => {
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const [tagValues, setTagValues] = useState<Record<string, boolean>>({});

  const { user } = useAuth();
  const { tags } = useUserData();

  useImperativeHandle(ref, () => ({
    close() {
      setIsOpen(false);
    },
    open() {
      setIsOpen(true);
    },
  }));

  const mutation = useMutation({
    mutationKey: ["batchTagEntry", user?.id, props.entry.ent_seq],
    mutationFn: async (variables: CommandEntryTagsProps) => {
      await commandEntryTags(variables);
    },
  });

  useEffect(() => {
    if (mutation.isError) {
      console.error(mutation.error);
    }
  }, [mutation]);

  const debouncedMutate = debounce((newSelectedTags) => {
    mutation.mutate({
      ent_seq: props.entry.ent_seq,
      tagValues: newSelectedTags,
    });
  }, 1000);

  useEffect(() => {
    // only if this entry is already tracked, then check which tags it's tracked by
    if (
      props.entry.trackedEntry &&
      props.entry.trackedEntry?.entryIsTaggeds.length !== 0
    ) {
      const alreadyChecked: Record<string, boolean> = {};
      const alreadyTags = props.entry.trackedEntry.entryIsTaggeds.flatMap(
        (eit) => eit.tag
      );
      for (let i = 0; i < alreadyTags.length; i++) {
        alreadyChecked[alreadyTags[i].id] = true;
      }
      setTagValues(alreadyChecked);
    }
  }, []);

  return (
    <>
      <div
        className={`w-screen h-screen fixed top-0 left-0 right-0 bottom-0
          bg-[rgba(0,0,0,0.7)] transition-opacity duration-300 z-10
          ${isOpen ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}
        onClick={() => setIsOpen(false)}
      >
        <Card
          className={`
            absolute top-[40%] left-[50%] -translate-x-1/2 -translate-y-1/2
            transition-all duration-300 transform
            ${isOpen ? "opacity-100 translate-y-0" : "opacity-0 translate-y-4"}`}
          onClick={(e) => e.stopPropagation()}
        >
          <CardHeader>
            <CardTitle>
              <h4 className="font-normal text-xl">Add word to...</h4>
            </CardTitle>
          </CardHeader>
          <CardContent>
            {tags?.map((tag, index) => {
              return (
                <div
                  key={index}
                  className="flex flex-row gap-4 mb-4 cursor-pointer"
                  onClick={() =>
                    setTagValues((prev) => {
                      const newTagValues = {
                        ...prev,
                        [tag.id]: !prev[tag.id],
                      };
                      debouncedMutate(newTagValues);
                      return newTagValues;
                    })
                  }
                >
                  <Checkbox checked={tagValues[tag.id] || false} />
                  <span className="text-md leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                    {tag.name}
                  </span>
                </div>
              );
            })}
          </CardContent>
        </Card>
      </div>
    </>
  );
});

export default TagsModal;
