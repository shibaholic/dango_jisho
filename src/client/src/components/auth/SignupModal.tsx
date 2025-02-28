import { Separator } from "@/components/ui/separator";
import React, {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useState,
} from "react";
import NavBar from "../header/NavBar";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { X } from "lucide-react";
import { AuthInput, Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useForm, SubmitHandler } from "react-hook-form";
import { useMutation, useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url } from "@/utils/api";
import { User, UserAuth, UserAuthSchema, UserSchema } from "@/types/User";

import axios, { AxiosError, AxiosResponse } from "axios";
import { useAuth } from "@/utils/auth";

interface ISignupInput {
  username: string;
  password: string;
}

const signupUser = async (data: ISignupInput): Promise<User> => {
  const response = await axios.post<User>(`${api_url}/auth/register`, data, {
    headers: {
      Accept: "*/*",
      "Content-Type": "application/json",
    },
    withCredentials: true,
  });

  return response.data;
};

export interface AuthModalHandle {
  close: () => void;
  open: () => void;
}

type StateType = {
  [key: string]: string;
};

const SignupModal = forwardRef((props, ref) => {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [errorMsgs, setErrorMsgs] = useState<StateType>({});
  const [isSuccess, setIsSuccess] = useState<boolean>(false);

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

  function clearErrorMsg() {
    setErrorMsgs({});
  }

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<ISignupInput>();

  const mutation = useMutation({
    mutationFn: signupUser,
    onMutate: () => {
      clearErrorMsg();
    },
    onSuccess: async (data) => {
      console.log("Signup success");

      setErrorMsgs({});
      setIsSuccess(true);
      reset();
    },
    onError: (error: unknown) => {
      console.error("Signup Mutation failed");
      if (axios.isAxiosError(error)) {
        // Server responded with status code not in 2xx range
        if (error.response) {
          console.error(
            "HTTP Error:",
            error.response.status,
            error.response.data
          );
          switch (error.response.status) {
            case 401:
              updateErrorMsg("axios", error.response.data as string);
              break;
            case 400:
              // user error
              console.log(error.response.data as string);
              updateErrorMsg("usernameTaken", error.response.data as string);
              break;
            default:
              updateErrorMsg("axios", error.response.data.title);
          }
        } else if (error.request) {
          // No response received (network issue, timeout, CORS issue, etc.)
          console.error("Network Error: ", error.message);
          updateErrorMsg("network", error.message);
        } else {
          // Unexpected error
          console.error("Unexpected Error: ", error.message);
          alert("Unexpected error: " + error.message);
        }
      } else {
        // non-axios error
        console.error("Unknown Error: ", error);
        alert("Unknown error");
      }
    },
  });

  const onSubmit: SubmitHandler<ISignupInput> = (data) => {
    mutation.mutate(data);
  };

  useEffect(() => {
    // delete all errorMsgs that start with formError_
    setErrorMsgs((prevState) =>
      Object.fromEntries(
        Object.entries(prevState).filter(
          ([key]) => !key.startsWith("formError_")
        )
      )
    );

    // add all errors found to errorMsgs, only if Signup error is not there.
    const errorEntries = Object.entries(errors);
    errorEntries.forEach((errorEntry) => {
      const [key, error] = errorEntry;
      updateErrorMsg("formError_" + key, error.message);
    });
  }, [errors.username, errors.password]); // [Object.entries(errors)]

  useEffect(() => {
    async function asyncEffect() {
      if (isOpen == false && isSuccess == true) {
        const delay = (ms) => new Promise((res) => setTimeout(res, ms));
        await delay(1000);
        console.log("delay over");
        setIsSuccess(false);
      }
    }
    asyncEffect();
  }, [isOpen]);

  useImperativeHandle(ref, () => ({
    close() {
      setIsOpen(false);
    },
    open() {
      setIsOpen(true);
    },
  }));

  let cardContents = <p>default</p>;
  if (!isSuccess) {
    cardContents = (
      <form className="flex flex-col gap-4" onSubmit={handleSubmit(onSubmit)}>
        <div className="auth-field">
          <Label htmlFor="username">Username</Label>
          <AuthInput
            id="username"
            {...register("username", {
              required: "Username is required",
            })}
          />
        </div>
        <div className="flex flex-col">
          <div className="auth-field">
            <Label htmlFor="password">Password</Label>
            <AuthInput
              id="password"
              type="password"
              {...register("password", {
                required: "Password is required",
              })}
            />
          </div>
        </div>

        {Object.entries(errorMsgs).map((errorMsg, index) => {
          return (
            <span key={index} className="fade-in-top text-red-500 self-center">
              {errorMsg[1]}
            </span>
          );
        })}

        <Button type="submit" variant="outline">
          Signup
        </Button>
      </form>
    );
  } else {
    cardContents = (
      <div className="flex flex-col text-center">
        <p className="text-xl">User successfully registered!</p>
        <p className="text-xl">Now just log in with the same credentials.</p>
        <Button
          className="mt-3"
          variant="outline"
          onClick={() => setIsOpen(false)}
        >
          Okay
        </Button>
      </div>
    );
  }

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
              <h2>Signup</h2>
            </CardTitle>
            <Button
              className="w-9 absolute top-0 right-1.5"
              onClick={() => setIsOpen(false)}
            >
              <X />
            </Button>
          </CardHeader>
          <CardContent>{cardContents}</CardContent>
        </Card>
      </div>
    </>
  );
});

export default SignupModal;
