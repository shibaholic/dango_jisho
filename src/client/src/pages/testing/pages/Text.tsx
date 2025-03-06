import React from "react";

const TextTestPage = () => {
  const text = "憂鬱アエイウオあえいおう系";

  const fontWeights = [
    "font-thin",
    "font-extralight",
    "font-light",
    "font-normal",
    "font-medium",
    "font-semibold",
    "font-bold",
    "font-extrabold",
    "font-black",
  ];

  return (
    <div lang="ja" className="text-6xl">
      {fontWeights.map((fontWeight, index) => {
        return <p className={`${fontWeight}`}>{text}</p>;
      })}
    </div>
  );
};

export default TextTestPage;
