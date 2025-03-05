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
              <span>Vocabulary </span>
              <span>
                {reviewingOrKnown} / {tag.totalEntries}
              </span>
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

const defaultColumns: ColumnDef<TrackedEntry>[] = [
  {
    accessorKey: "ent_seq",
    header: "ent_seq",
  },
  {
    accessorFn: (row) => {
      if (row.entry.kanjiElements.length > 0)
        return row.entry.kanjiElements[0].keb;
    },
    header: "Kanji",
  },
  {
    accessorFn: (row) => {
      return row.entry.readingElements[row.entry.selectedReadingIndex].reb;
    },
    header: "Reading",
  },
];

const TagWords = ({ tag }: TagWordsProps) => {
  // State to manage whether the table is expanded
  const [isExpanded, setIsExpanded] = useState(false);

  const [pagination, setPagination] = useState({
    pageIndex: 0,
    pageSize: 10,
  });

  const { data, isLoading, isFetching, isError, error, isPlaceholderData } =
    useQuery({
      queryKey: ["tag-words", tag.id, pagination.pageIndex],
      queryFn: async () =>
        await fetchTagTrackedEntries(
          tag.id,
          pagination.pageIndex,
          pagination.pageSize
        ),
      placeholderData: keepPreviousData,
    });

  // Create the table instance using TanStack Table's hook
  const table = useReactTable({
    data: data?.data || [],
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onPaginationChange: setPagination,
    manualPagination: true,
    state: { pagination },
    rowCount: data?.totalElements || 10,
  });

  let tableElement = isLoading ? (
    <div>Loading...</div>
  ) : isError ? (
    <div>Error: {error.message}</div>
  ) : (
    <>
      <div
        className={`relative mx-4 ${isExpanded ? "max-h-full" : "max-h-32 overflow-hidden cursor-pointer"}`}
      >
        {!isExpanded && (
          <>
            <div className="absolute top-0 left-0 w-full h-32 bg-gradient-to-b from-transparent to-white pointer-events-none flex justify-center z-10">
              <span className="self-end">Click to expand</span>
            </div>
          </>
        )}
        <Table className="min-w-full">
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead key={header.id}>
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows.map((row) => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <div className="flex items-center justify-between space-x-2 py-4">
          <div>
            Words {pagination.pageIndex * pagination.pageSize + 1} -{" "}
            {(pagination.pageIndex + 1) * pagination.pageSize >
            data!.totalElements!
              ? data!.totalElements!
              : (pagination.pageIndex + 1) * 10}
          </div>
          <div>
            Page {pagination.pageIndex + 1}/{""}
            {Math.ceil(data!.totalElements! / pagination.pageSize)}
          </div>
          <div className="leading-none flex">
            <Button
              variant="outline"
              size="sm"
              onClick={() => table.previousPage()}
              disabled={pagination.pageIndex === 0}
            >
              Previous
            </Button>
            <Button
              className="w-[50px]"
              variant="outline"
              size="sm"
              onClick={() => table.nextPage()}
              disabled={
                isPlaceholderData ||
                (pagination.pageIndex + 1) * pagination.pageSize >=
                  data!.totalElements! ||
                isFetching
              }
            >
              {isFetching ? <l-ring size="20" stroke="2" /> : "Next"}
            </Button>
          </div>
        </div>
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
