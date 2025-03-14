export function convertTimeSpanToMs(timeSpan: string) {
  const [hours, minutes, seconds] = timeSpan.split(":").map(Number);
  const timeSpanMs = ((hours * 60 + minutes) * 60 + seconds) * 1000;

  return timeSpanMs;
}

export function humanizeDelta(deltaMs: number): string {
  // Define the time unit constants (in milliseconds)
  const second = 1000;
  const minute = 60 * second;
  const hour = 60 * minute;
  const day = 24 * hour;
  const month = 30 * day; // Approximate month as 30 days
  const year = 365 * day; // Approximate year as 365 days

  if (deltaMs < minute) {
    const seconds = Math.floor(deltaMs / second);
    return seconds + (seconds === 1 ? " second" : " seconds");
  } else if (deltaMs < hour) {
    const minutes = Math.floor(deltaMs / minute);
    return minutes + (minutes === 1 ? " minute" : " minutes");
  } else if (deltaMs < day) {
    const hours = Math.floor(deltaMs / hour);
    return hours + (hours === 1 ? " hour" : " hours");
  } else if (deltaMs < month) {
    const days = Math.floor(deltaMs / day);
    return days + (days === 1 ? " day" : " days");
  } else if (deltaMs < year) {
    const months = Math.floor(deltaMs / month);
    return months + (months === 1 ? " month" : " months");
  } else {
    // In case the delta is one year or more, we display years.
    const years = Math.floor(deltaMs / year);
    return years + (years === 1 ? " year" : " years");
  }
}
