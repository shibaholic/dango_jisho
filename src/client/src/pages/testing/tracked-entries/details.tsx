import { api_url } from "@/Api";
import { useQuery } from "@tanstack/react-query";
import {
  createContext,
  forwardRef,
  useContext,
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
// import CardBackSmall from "../card-back/card-back";

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

      {/* <HeaderValue header={"ent_seq"} field={ent_seq} /> */}

      <div key={ent_seq}>
        {ent_seq ? (
          <>
            <Section
              title="TrackedEntry details"
              queryKeyName="trackedEntry"
              ent_seq={ent_seq}
              fetchFn={fetch(`${api_url}/TrackedEntry/id?ent_seq=${ent_seq}`)}
            >
              <HeaderValue header="ent_seq" field="ent_seq" />
              <HeaderValue header="User ID" field="userId" />
              <HeaderValue
                header="Level State"
                field="levelStateType"
                modifier={levelStateToString}
              />
              <HeaderValue
                header="Old Level State"
                field="oldLevelStateType"
                modifier={levelStateToString}
              />
              <HeaderValue
                header="Special Category"
                field="specialCategory"
                modifier={specialCategoryToString}
              />
              <HeaderValue header="Last Review Date" field="lastReviewDate" />
              <HeaderValue header="Next Review Days" field="nextReviewDays" />
              <HeaderValue
                header="Next Review Minutes"
                field="nextReviewMinutes"
              />
            </Section>
            <Section
              title="Entry details"
              queryKeyName="entry"
              ent_seq={ent_seq}
              fetchFn={fetch(`${api_url}/entry?ent_seq=${ent_seq}`)}
            >
              {/**
               * If any entry.kanjiElements, then start new <div> with padding-left for indentation, with <p header> Kanjis </p>
               * For every kanjiElement, <HeaderValue> kanjiElement properties, and comma-separated for string[], margin-down for spacing
               * Basically, repeat for reading, sense and lsource inside sense.
               *  */}
              <p>TODO Fill with entry</p>
            </Section>
          </>
        ) : (
          <p>Click "Show details"</p>
        )}
      </div>
    </>
  );
});

interface SectionContextProps<T> {
  data: T;
  // setData: React.Dispatch<React.SetStateAction<T>>;
}

const SectionContext = createContext<SectionContextProps<any> | undefined>(
  undefined
);

const useSectionContext = <T,>() => {
  const context = useContext<SectionContextProps<T> | undefined>(
    SectionContext
  );
  if (!context) {
    throw new Error("useSectionContext must be used within a SectionProvider");
  }
  return context;
};

interface SectionProps<T extends Response> {
  title: string;
  ent_seq: string;
  queryKeyName: string;
  fetchFn: Promise<T>;
  children?: React.ReactNode;
}

function Section<T extends Response>({
  title,
  ent_seq,
  queryKeyName,
  fetchFn,
  children,
}: SectionProps<T>): React.JSX.Element {
  const [data, setData] = useState<T | null>(null);
  const [status, setStatus] = useState<string>("undefined");
  const [open, setOpen] = useState<boolean>(false);

  const { refetch, isError, error } = useQuery({
    queryKey: [queryKeyName, ent_seq],
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
            <SectionContext.Provider value={{ data }}>
              {children}
            </SectionContext.Provider>
          )}
        </CollapsibleContent>

        <Separator className="my-4" />
      </Collapsible>
    </>
  );
}

interface HeaderValueProps<T> {
  header: string;
  // field: string | null | undefined;
  field: keyof T;
  modifier?: (arg0: any) => string;
}

const HeaderValue = <T,>({ header, field, modifier }: HeaderValueProps<T>) => {
  const { data } = useSectionContext<T>();

  return (
    <>
      <p className="mb-4 font-medium">
        {header}:{" "}
        <span className="font-normal font-mono">
          {data[field]
            ? modifier
              ? modifier(data[field])
              : data[field] + ""
            : "null"}
        </span>
      </p>
    </>
  );
};
