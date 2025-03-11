import { ReactNode } from "react";

interface DropdownRowProps {
  children?: ReactNode; // icons
  text: string;
  action: () => void;
}

const DropdownRow = ({ children, text, action }: DropdownRowProps) => {
  return (
    <div
      className="flex flex-row px-4 py-2 hover:bg-gray-100 hover:text-[#535bf2] cursor-pointer items-center rounded-lg"
      onClick={action}
    >
      {children}
      <span className="font-medium block ml-2 text-sm">{text}</span>
    </div>
  );
};

export { DropdownRow };
