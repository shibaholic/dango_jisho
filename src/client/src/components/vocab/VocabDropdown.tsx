import { Menu, Tag } from "lucide-react";
import {
  forwardRef,
  useState,
  useRef,
  useEffect,
  useImperativeHandle,
} from "react";
import { DropdownRow } from "../dropdown/Dropdown";
import { Button } from "../ui/button";
import { createPortal } from "react-dom";

export interface VocabDropdownProps {
  openTagsModal: () => void;
}

export const VocabDropdown = ({ openTagsModal }: VocabDropdownProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const [position, setPosition] = useState({ top: 0, left: 0, width: 0 });
  const buttonRef = useRef<HTMLButtonElement>(null);

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

  useEffect(() => {
    if (isOpen && buttonRef.current) {
      const rect = buttonRef.current.getBoundingClientRect();
      const width = rect.width;
      const top = rect.bottom + window.scrollY + 8;
      const left = rect.left + window.scrollX - width * 2 - 2;
      setPosition({
        top: top, // Position below the button
        left: left, // Align left
        width: rect.width,
      });
    }
  }, [isOpen]);

  function openDropdown() {
    setIsOpen(true);
  }

  return (
    <div className="relative">
      <Button
        ref={buttonRef}
        onClick={() => openDropdown()}
        variant="outline"
        className="w-1"
      >
        <Menu />
      </Button>

      {isOpen &&
        createPortal(
          <div
            ref={dropdownRef}
            className={
              "absolute rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 z-100"
            } // absolute right-0 mt-2 w-48
            style={{
              top: position.top,
              left: position.left,
              // width: position.width,
              position: "absolute",
            }}
            role="menu"
            aria-orientation="vertical"
          >
            <div className="py-1 px-1.5">
              <DropdownRow
                text="Tags"
                action={() => {
                  openTagsModal();
                }}
              >
                <Tag />
              </DropdownRow>
            </div>
          </div>,
          document.body
        )}
    </div>
  );
};
