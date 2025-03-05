import { User } from "@/types/User";
import { LogOut, Settings, User as UserIcon, UserPen } from "lucide-react";
import React, {
  ReactNode,
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import { Separator } from "../ui/separator";
import { useAuth } from "@/utils/auth";
import { fetchLogout } from "@/utils/api";
import { useNavigate } from "react-router-dom";

export interface UserBadgeProps {
  user: User;
}

export const UserBadge = ({ user }: UserBadgeProps) => {
  const dropdownRef = useRef<UserBadgeDropdownRef>(null);

  return (
    <div className="relative">
      <div
        className="rounded-xl border py-[10px] pl-2 pr-4 cursor-pointer hover:bg-gray-100"
        onClick={() => dropdownRef.current?.openDropdown()}
      >
        <span className="flex flex-row gap-2 font-medium text-xl ">
          <UserIcon />
          {user!.username}
        </span>
      </div>
      <UserBadgeDropdown ref={dropdownRef} />
    </div>
  );
};

export interface UserBadgeDropdownRef {
  openDropdown: () => void;
  closeDropdown: () => void;
}

const UserBadgeDropdown = forwardRef<UserBadgeDropdownRef>((_, ref) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const { setUser } = useAuth();

  const navigate = useNavigate();

  // Handle clicks outside to close the dropdown
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  // Expose open and close functions to parent via ref
  useImperativeHandle(ref, () => ({
    openDropdown: () => setIsOpen(true),
    closeDropdown: () => setIsOpen(false),
  }));

  async function clickLogout() {
    await fetchLogout();

    // set user client-side to null
    setUser(null);

    // then depending on client-side authorization routing navigate somewhere or refresh page
    navigate(0);
  }

  if (!isOpen) return null;

  return (
    <div
      ref={dropdownRef}
      className="absolute right-1 mt-2 w-48 rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 z-10"
      role="menu"
      aria-orientation="vertical"
    >
      <div className="py-1 px-1.5">
        <DropdownRow text="Profile" action={() => {}}>
          <UserPen />
        </DropdownRow>

        <DropdownRow text="Settings" action={() => {}}>
          <Settings />
        </DropdownRow>

        <Separator />

        <DropdownRow text="Log out" action={clickLogout}>
          <LogOut />
        </DropdownRow>
      </div>
    </div>
  );
});

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
