import { api_url } from "@/Api";
import { useQuery } from "@tanstack/react-query";
import {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";

import { ChevronsUpDown, ArrowDownToLine } from "lucide-react";
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

  useImperativeHandle(ref, () => ({
    setES: (es: string) => {
      setEnt_seq(es);
    },
  }));

  return (
    <>
      {/* <Button onClick={testPrint}>Test Print</Button> */}

      <HeaderValue header={"ent_seq"} value={ent_seq} />

      <div key={ent_seq}>
        {ent_seq ? (
          <Section
            title="Entry details"
            ent_seq={ent_seq}
            fetchFn={fetch(`${api_url}/TrackedEntry/id?ent_seq=${ent_seq}`)}
          />
        ) : (
          <p>Click "Show details"</p>
        )}
      </div>
    </>
  );
});

interface SectionProps<T extends Response> {
  title: string;
  ent_seq: string;
  fetchFn: Promise<T>;
}

function Section<T extends Response>({
  title,
  ent_seq,
  fetchFn,
}: SectionProps<T>) {
  const [data, setData] = useState<TrackedEntry | null>(null);
  const [status, setStatus] = useState<string>("undefined");
  const [open, setOpen] = useState<boolean>(false);

  const { refetch, isError, error } = useQuery({
    queryKey: ["trackedEntry", ent_seq],
    queryFn: async () => {
      let result = await fetchFn;
      if (!result.ok) throw new Error("Network response was not ok");
      return result.json();
    },
    enabled: false,
  });

  async function loadSection() {
    const freshData = await refetch();

    if (isError) {
      setStatus("data error");
      console.error(error);
    } else {
      if (freshData.data) {
        setData(freshData.data.data);
        setStatus("loaded");
        setOpen(true);
      }
    }
  }

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

        <CollapsibleContent>
          {data && (
            <>
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
            </>
          )}
        </CollapsibleContent>

        <Separator className="my-4" />
      </Collapsible>
    </>
  );
}

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
