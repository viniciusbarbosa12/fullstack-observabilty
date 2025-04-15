// src/utils/dateFormatter.ts

import { format, intervalToDuration } from "date-fns";

export function formatDateWithDiff(dateString: string): string {
  const date = new Date(dateString);
  const now = new Date();
  const duration = intervalToDuration({ start: date, end: now });

  const formattedDate = format(date, "MMMM d, yyyy");
  const diff = `(${duration.years ?? 0}y – ${duration.months ?? 0}m – ${
    duration.days ?? 0
  }d)`;

  return `${formattedDate} ${diff}`;
}
