import { api_url } from "@/Api";
import { useQuery } from "@tanstack/react-query";
import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";

import { ChevronsUpDown } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  LevelState,
  TrackedEntry,
  levelStateToString,
  specialCategoryToString,
} from "@/types/TrackedEntry";

export interface DetailsRef {
  // fetchData: (es: string) => void;
  setES: (es: string) => void;
}

export const Details = forwardRef((props, ref: React.Ref<DetailsRef>) => {
  const [ent_seq, setEnt_seq] = useState<string | null>(null);
  const [data, setData] = useState<TrackedEntry | null>(null);
  const [status, setStatus] = useState<string>("undefined");

  async function fetchData() {
    const freshData = await refetch();

    if (isError) {
      setStatus("data error");
      console.error(error);
    } else {
      if (freshData.data) {
        setData(freshData.data.data);
        setStatus("loaded");
      }
    }
  }

  useImperativeHandle(ref, () => ({
    setES: (es: string) => {
      setEnt_seq(es);
    },
  }));

  const { refetch, isError, error } = useQuery({
    queryKey: ["trackedEntry", ent_seq],
    queryFn: async () => {
      let result = await fetch(`${api_url}/TrackedEntry/id?ent_seq=${ent_seq}`);
      if (!result.ok) throw new Error("Network response was not ok");
      return result.json();
    },
    enabled: false,
  });

  useEffect(() => {
    if (ent_seq != null) {
      fetchData();
    }
  }, [ent_seq]);

  // function testPrint() {
  //   console.log(levelStateToString(data!.levelStateType));
  // }

  return (
    <>
      {/* <Button onClick={testPrint}>Test Print</Button> */}

      <HeaderValue header={"ent_seq"} value={ent_seq} />

      {status === "loaded" && data ? (
        <Collapsible className="w-full">
          <div className="flex w-full mb-4 gap-4 items-center">
            <h4 className="max-w-1/2 text-md font-bold">Entry details</h4>
            <CollapsibleTrigger asChild className="flex-grow">
              <Button variant="outline" size="sm">
                <ChevronsUpDown className="h-full w-full" />
                <span className="sr-only">Toggle</span>
              </Button>
            </CollapsibleTrigger>
          </div>

          <CollapsibleContent>
            <HeaderValue header={"User ID"} value={data.userId} />
            <HeaderValue
              header={"Level State"}
              value={levelStateToString(data.levelStateType)}
            />
            <HeaderValue
              header={"Old Level State"}
              value={levelStateToString(data.oldLevelStateType)}
            />
            <HeaderValue
              header={"Special Category"}
              value={specialCategoryToString(data.specialCategory)}
            />
            <HeaderValue
              header={"Last Review Date"}
              value={data.lastReviewDate?.toString()}
            />
            <HeaderValue
              header={"Next Review Days"}
              value={data.nextReviewDays?.toString()}
            />
            <HeaderValue
              header={"Next Review Minutes"}
              value={data.nextReviewMinutes?.toString()}
            />
          </CollapsibleContent>

          <Separator className="my-4" />
        </Collapsible>
      ) : (
        <p>{status}</p>
      )}
    </>
  );
});

interface HeaderValueProps {
  header: string;
  value: string | null | undefined;
}

const HeaderValue = ({ header: key, value }: HeaderValueProps) => {
  return (
    <>
      <p className="mb-4 font-medium">
        {key}:{" "}
        <span className="font-normal font-mono">{value ? value : "null"}</span>
      </p>
    </>
  );
};
