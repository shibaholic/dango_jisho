import React from "react";

type ObjectDisplayProps = {
  data: any;
  label?: string;
};

const ObjectDisplay: React.FC<ObjectDisplayProps> = ({ data, label }) => {
  // Handle null or undefined values:
  if (data === null) {
    return (
      <div>
        {label ? <strong className="text-yellow-700">{label}:</strong> : ""}{" "}
        null
      </div>
    );
  }

  if (typeof data === "undefined") {
    return (
      <div>
        {label ? <strong className="text-red-700">{label}:</strong> : ""}{" "}
        undefined
      </div>
    );
  }

  // If it's a Date, format it:
  if (data instanceof Date) {
    return (
      <div>
        {label ? `${label}: ` : ""}
        {data.toLocaleString()}
      </div>
    );
  }

  // If it's an array, recursively display each element:
  if (Array.isArray(data)) {
    if (data.length > 0) {
      return (
        <div>
          {label && (
            <strong className="text-blue-500">
              {label} {"["}
            </strong>
          )}
          <div className="pl-4 border-l-[1px] border-blue-500">
            {data.map((item, index) => (
              <ObjectDisplay key={index} data={item} />
            ))}
          </div>
          <strong className="text-blue-500">{"]"}</strong>
        </div>
      );
    } else {
      return (
        <div>
          {label && (
            <strong className="text-blue-500">
              {label} {"[]"}
            </strong>
          )}
        </div>
      );
    }
  }

  // If it's an object, recursively display its keys/values:
  if (typeof data === "object") {
    return (
      <div>
        {label && <strong className="text-green-500">{label} </strong>}
        <strong className="text-green-500">{"{"}</strong>
        <div className="pl-4 border-l-[1px] border-green-500">
          {Object.entries(data).map(([key, value]) => (
            <ObjectDisplay key={key} data={value} label={key} />
          ))}
        </div>
        <strong className="text-green-500">{"}"}</strong>
      </div>
    );
  }

  // For primitive types (number, string, boolean), simply display them:
  return (
    <div>
      {label ? <strong className="text-yellow-500">{`${label}: `}</strong> : ""}
      {data.toString()}
    </div>
  );
};

export default ObjectDisplay;
