"use client";

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { forwardRef, useImperativeHandle, useState } from "react";

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
}

export interface TableRef<TData> {
  getSelectedRow: () => TData | null;
  getColumnValue: (columnKey: keyof TData) => TData[keyof TData] | null;
}

export const DataTable = forwardRef<TableRef<any>, DataTableProps<any, any>>(
  <TData, TValue>(
    { columns, data }: DataTableProps<TData, TValue>,
    ref: React.Ref<TableRef<TData>>
  ) => {
    const [selectedRow, setSelectedRow] = useState<TData | null>(null);
    const [selectedRowId, setSelectedRowId] = useState<string | null>(null);

    useImperativeHandle(ref, () => ({
      getSelectedRow: () => selectedRow,
      getColumnValue: (columnKey) => {
        if (!selectedRow) return null;
        return selectedRow[columnKey];
      },
    }));

    const table = useReactTable({
      data,
      columns,
      getCoreRowModel: getCoreRowModel(),
    });

    return (
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                            header.column.columnDef.header,
                            header.getContext()
                          )}
                    </TableHead>
                  );
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                  onClick={(e) => {
                    const selection = window.getSelection();
                    selection && selection.toString()
                      ? e.preventDefault()
                      : (setSelectedRow(row.original),
                        setSelectedRowId(row.id));
                  }}
                  style={{
                    backgroundColor:
                      row.id === selectedRowId ? "lightblue" : "white",
                  }}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    );
  }
);
