import {
  CommandEntryTagsProps,
  commandEntryTags,
  commandNewTag,
} from "@/utils/api";
import { useAuth } from "@/utils/auth";
import { debounce } from "@/utils/debounce";
import { useUserData } from "@/utils/userDataProvider";
import { Checkbox } from "@radix-ui/react-checkbox";
import { useMutation } from "@tanstack/react-query";
import { forwardRef, useState, useImperativeHandle, useEffect } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "../ui/card";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Label } from "@radix-ui/react-label";
import { SubmitHandler, useForm } from "react-hook-form";

type StateType = {
  [key: string]: string;
};

interface NewTagModalProps {
  refreshTags: () => void;
}

export interface NewTagModalHandle {
  close: () => void;
  open: () => void;
}

const NewTagModal = forwardRef<NewTagModalHandle, NewTagModalProps>(
  (props, ref) => {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [errorMsgs, setErrorMsgs] = useState<StateType>({});

    const [name, setName] = useState<string>("");

    const { user } = useAuth();
    const { refreshTags: refreshUserTags } = useUserData();

    useImperativeHandle(ref, () => ({
      close() {
        setIsOpen(false);
      },
      open() {
        setIsOpen(true);
      },
    }));

    const mutation = useMutation({
      mutationKey: ["new-tag", user?.id, name],
      mutationFn: async (data: NewTagInput) => {
        await commandNewTag(data);
      },
      onSuccess: () => {
        console.log("Create new tag success");
        // console.log(data);

        // TODO: optimistic updating of Tags data.

        // instead we will just refetch tags
        props.refreshTags();
        refreshUserTags();
        setErrorMsgs({});
        reset();

        setIsOpen(false);
      },
    });

    useEffect(() => {
      if (mutation.isError) {
        console.error(mutation.error);
      }
    }, [mutation]);

    const {
      register,
      handleSubmit,
      reset,
      formState: { errors },
    } = useForm<NewTagInput>();

    interface NewTagInput {
      name: string;
    }

    const onSubmit: SubmitHandler<NewTagInput> = (data) => {
      mutation.mutate(data);
    };

    function updateErrorMsg(key: string, value?: string) {
      if (!value) {
        if (errorMsgs[key]) delete errorMsgs[key];
      } else {
        setErrorMsgs((prevState) => ({
          ...prevState,
          [key]: value, // Dynamically adding/updating key-value pairs
        }));
      }
    }

    useEffect(() => {
      // delete all errorMsgs that start with formError_
      setErrorMsgs((prevState) =>
        Object.fromEntries(
          Object.entries(prevState).filter(
            ([key]) => !key.startsWith("formError_")
          )
        )
      );

      // add all errors found to errorMsgs, only if login error is not there.
      const errorEntries = Object.entries(errors);
      errorEntries.forEach((errorEntry) => {
        const [key, error] = errorEntry;
        updateErrorMsg("formError_" + key, error.message);
      });
    }, [errors]);

    return (
      <>
        <div
          className={`w-screen h-screen fixed top-0 left-0 right-0 bottom-0
          bg-[rgba(0,0,0,0.7)] transition-opacity duration-300 z-10
          ${isOpen ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}
          onClick={() => setIsOpen(false)}
        >
          <Card
            className={`
            absolute top-[40%] left-[50%] -translate-x-1/2 -translate-y-1/2
            transition-all duration-300 transform
            ${isOpen ? "opacity-100 translate-y-0" : "opacity-0 translate-y-4"}`}
            onClick={(e) => e.stopPropagation()}
          >
            <CardHeader>
              <CardTitle>
                <h4 className="font-normal text-xl">Create a new tag...</h4>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <form
                className="flex flex-col gap-4"
                onSubmit={handleSubmit(onSubmit)}
              >
                <div className="auth-field text-nowrap">
                  <Label htmlFor="name" className="font-medium">
                    Tag name
                  </Label>
                  <Input
                    id="name"
                    type="text"
                    {...register("name", {
                      required: "Tag name is required",
                    })}
                  />
                </div>

                {Object.entries(errorMsgs).map((errorMsg, index) => {
                  return (
                    <span
                      key={index}
                      className="fade-in-top text-red-500 self-center"
                    >
                      {errorMsg[1]}
                    </span>
                  );
                })}

                <Button type="submit" variant="outline">
                  Create
                </Button>
              </form>
            </CardContent>
          </Card>
        </div>
      </>
    );
  }
);

export default NewTagModal;
