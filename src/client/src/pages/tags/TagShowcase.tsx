import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Entry, Sense } from "@/types/JMDict";
import { Tag } from "@/types/Tag";
import { TrackedEntry } from "@/types/TrackedEntry";
import { ApiResponse, fetchTagTrackedEntries } from "@/utils/api";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  getPaginationRowModel,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";

import { ring } from "ldrs";
import { FuriganaSegment, annotateFurigana } from "@/utils/furigana";
import { useNavigate } from "react-router-dom";
import SmallListCard from "@/components/vocab/SmallListCard";
import { Separator } from "@/components/ui/separator";
ring.register();

export interface TagShowcaseProps {
  tag: Tag;
}

// This component is used in Tags.tsx, and normally shows a summary of this Tag,
// by only showing a few Entries within it.
// But the user can click to expand it and query the Tag's Entries more in depth.
const TagShowcase = ({ tag }: TagShowcaseProps) => {
  const reviewingOrKnown = tag.totalReviewing + tag.totalKnown;
  const percentage = (reviewingOrKnown / tag.totalEntries) * 100;

  return (
    <Card>
      <CardHeader className="flex flex-row justify-between space-y-0">
        <CardTitle>
          <h3>{tag.name}</h3>
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div>
          <div>
            <div className="flex flex-row justify-between">
              <div>Vocabulary progress</div>
              <div>
                {reviewingOrKnown} / {tag.totalEntries}
              </div>
            </div>
          </div>
          <Progress value={percentage} />
          <TagWords tag={tag} />
        </div>
      </CardContent>
    </Card>
  );
};

export default TagShowcase;

interface TagWordsProps {
  tag: Tag;
}

const defaultColumns: ColumnDef<TrackedEntry>[] = [];

const TagWords = ({ tag }: TagWordsProps) => {
  const [isExpanded, setIsExpanded] = useState(false);

  const [pagination, setPagination] = useState({
    pageIndex: 0,
    pageSize: 10,
  });

  const [tableData, setTableData] = useState<TrackedEntry[]>([]);

  useEffect(() => {
    setTableData(
      tag.entryIsTaggeds
        .map((eit) => eit.trackedEntry)
        .filter((entry): entry is TrackedEntry => entry !== null)
    );
  }, []);

  // const { data, isLoading, isFetching, isError, error, isPlaceholderData } =
  //   useQuery({
  //     queryKey: ["tag-words", tag.id, pagination.pageIndex],
  //     queryFn: async () =>
  //       await fetchTagTrackedEntries(
  //         tag.id,
  //         pagination.pageIndex,
  //         pagination.pageSize
  //       ),
  //     placeholderData: keepPreviousData,
  //   });

  // Create the table instance using TanStack Table's hook
  const table = useReactTable({
    data: tableData,
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onPaginationChange: setPagination,
    // manualPagination: true,
    state: { pagination },
    // rowCount: pagination.pageSize, // data?.totalElements || 10,
  });

  let tableElement = (
    <>
      <div
        className={`relative flex flex-col w-full ${isExpanded ? "max-h-full" : "max-h-32 overflow-hidden cursor-pointer"}`}
      >
        {!isExpanded && (
          <>
            <div className="absolute top-0 left-0 w-full h-full bg-gradient-to-b from-transparent to-white flex justify-center z-20">
              <span className="self-end">Click to expand</span>
            </div>
          </>
        )}

        {isExpanded && (
          <>
            <div className="flex items-center justify-between space-x-2 py-2 px-2">
              <div>
                Words {pagination.pageIndex * pagination.pageSize + 1} -{" "}
                {(pagination.pageIndex + 1) * pagination.pageSize >
                tableData.length
                  ? tableData.length
                  : (pagination.pageIndex + 1) * 10}
              </div>
              <PaginationButtons
                previousPage={() => table.previousPage()}
                previousDisabled={pagination.pageIndex === 0}
                nextPage={() => table.nextPage()}
                nextDisabled={
                  (pagination.pageIndex + 1) * pagination.pageSize >=
                  tableData.length
                }
              />
              <div>
                Page {pagination.pageIndex + 1}/{""}
                {Math.ceil(tableData.length / pagination.pageSize)}
              </div>
            </div>
            <Separator />
          </>
        )}

        <Table className="flex flex-col w-full overflow-hidden">
          <TableBody className="flex flex-col w-full">
            {table.getRowModel().rows.map((row, index, array) => {
              if (
                array.length - 1 === index &&
                pagination.pageSize !== array.length
              ) {
                const ghostRowCount = array.length;

                const ghostRows: JSX.Element[] = [
                  <SmallListCard key={row.id} trackedEntry={row.original} />,
                ];

                for (let i = array.length; i < pagination.pageSize; i++) {
                  ghostRows[i] = (
                    <TableRow
                      key={i}
                      className="flex w-full h-[98.9333px] pointer-events-none"
                    ></TableRow>
                  );
                }
                return ghostRows;
              }

              return <SmallListCard key={row.id} trackedEntry={row.original} />;
            })}
          </TableBody>
        </Table>

        <Separator />

        {isExpanded && (
          <div className="flex items-center justify-between space-x-2 py-2 px-2">
            <div>
              Words {pagination.pageIndex * pagination.pageSize + 1} -{" "}
              {(pagination.pageIndex + 1) * pagination.pageSize >
              tableData.length
                ? tableData.length
                : (pagination.pageIndex + 1) * 10}
            </div>
            <PaginationButtons
              previousPage={() => table.previousPage()}
              previousDisabled={pagination.pageIndex === 0}
              nextPage={() => table.nextPage()}
              nextDisabled={
                (pagination.pageIndex + 1) * pagination.pageSize >=
                tableData.length
              }
            />
            <div>
              Page {pagination.pageIndex + 1}/{""}
              {Math.ceil(tableData.length / pagination.pageSize)}
            </div>
          </div>
        )}
      </div>
    </>
  );

  return (
    // Click handler to expand the table. You might want to disable click events inside the table once expanded.
    <div
      onClick={() => setIsExpanded(true)}
      className={`transition-all duration-300 border rounded-md mt-4 py-2`}
    >
      <h4 className="ml-3 font-medium text-lg">Word list</h4>
      {tableElement}
    </div>
  );
};

interface PaginationButtonsProps {
  previousPage: () => void;
  previousDisabled: boolean;
  nextPage: () => void;
  nextDisabled: boolean;
}

export const PaginationButtons = ({
  previousPage,
  previousDisabled,
  nextPage,
  nextDisabled,
}: PaginationButtonsProps) => {
  return (
    <div className="flex">
      <Button
        variant="outline"
        size="sm"
        onClick={previousPage}
        disabled={previousDisabled}
      >
        Previous
      </Button>
      <Button
        className="w-[50px]"
        variant="outline"
        size="sm"
        onClick={nextPage}
        disabled={nextDisabled}
      >
        {/* {isFetching ? <l-ring size="20" stroke="2" /> : "Next"} */}
        Next
      </Button>
    </div>
  );
};
