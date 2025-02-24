import { ApiResponse, api_url } from "@/utils/api";
import { useQuery } from "@tanstack/react-query";
import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";

import { ChevronsUpDown, ArrowDownToLine } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { TrackedEntry, TrackedEntrySchema } from "@/types/TrackedEntry";
import ObjectDisplay from "./ObjectDisplay";
import { Entry, EntrySchema } from "@/types/JMDict";
import { Card, CardSchema } from "@/types/Card";
// import CardBackSmall from "../card-back/card-back";

export interface SectionProps<T> {
  title: string;
  ent_seq: string;
  queryKeyName: string;
  queryFn: () => Promise<T>;
}

export const DetailsSection = <T,>({
  title,
  ent_seq,
  queryKeyName,
  queryFn,
}: SectionProps<T>) => {
  const [data, setData] = useState<T | null>(null);
  const [status, setStatus] = useState<string>("undefined");
  const [open, setOpen] = useState<boolean>(true);

  const { refetch, isError, error } = useQuery({
    queryKey: [queryKeyName, ent_seq],
    queryFn: queryFn,
    enabled: false,
  });

  async function loadSection() {
    const freshData = await refetch();

    if (isError) {
      setStatus("load data error");
      console.error(error.message);
    } else {
      if (freshData.data) {
        setData(freshData.data);
        setStatus("loaded");
        // setOpen(true);
      }
    }
  }

  useEffect(() => {
    loadSection();
  }, []);

  return (
    <>
      <Collapsible className="w-full" open={open}>
        <div className="flex w-full mb-4 gap-4 items-center">
          <h4 className="max-w-1/2 text-md font-bold">{title}</h4>
          {status === "loaded" ? (
            <CollapsibleTrigger asChild className="flex-grow">
              <Button
                onClick={(e) => {
                  e.preventDefault();
                  setOpen(!open);
                }}
                variant="outline"
                size="sm"
              >
                <ChevronsUpDown className="h-full w-full" />
                <span className="sr-only">Toggle</span>
              </Button>
            </CollapsibleTrigger>
          ) : (
            <Button
              onClick={loadSection}
              variant="outline"
              className="flex-grow"
              size="sm"
            >
              <ArrowDownToLine className="h-full w-full" />
              <span className="sr-only">Fetch</span>
            </Button>
          )}
        </div>

        <CollapsibleContent className="font-mono">
          {data && <ObjectDisplay label={queryKeyName} data={data} />}
        </CollapsibleContent>

        <Separator className="my-4" />
      </Collapsible>
    </>
  );
};
