import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";

type FormData = {
  // Your form fields types here
  email: string;
  password: string;
};

type StateType = { [key: string]: string };

export default function RHKTestPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>();
  const [errorMsgs, setErrorMsgs] = useState<StateType>({});

  // Sync errors to state
  useEffect(() => {
    console.log("errors changed");

    const newErrors: StateType = {};

    Object.entries(errors).forEach(([fieldName, error]) => {
      if (error && error.message) {
        newErrors[fieldName] = error.message;
      }
    });

    setErrorMsgs(newErrors);
  }, [errors.email, errors.password]);

  console.log(errors);

  // Render error messages
  const renderErrors = () => (
    <div className="error-messages">
      {Object.entries(errorMsgs).map(([field, message]) => (
        <p key={field} className="error-message">
          {message}
        </p>
      ))}
    </div>
  );

  return (
    <form onSubmit={handleSubmit((data) => console.log(data))}>
      {/* Example input field */}
      <input
        {...register("email", {
          required: "Email is required",
          pattern: {
            value: /^\S+@\S+$/i,
            message: "Invalid email format",
          },
        })}
      />

      {/* Render errors */}
      {renderErrors()}

      <button type="submit">Submit</button>
    </form>
  );
}
