import { useQuery } from "@tanstack/react-query";
import { ISO8601String, toIsoStr } from "../utils/datetime";
import ky, { HTTPError } from "ky";
import { z } from "zod";

export interface NewReservation {
  RoomNumber: string;
  GuestEmail: string;
  Start: ISO8601String;
  End: ISO8601String;
}

/** The schema the API returns */
const ReservationSchema = z.object({
  Id: z.string(),
  RoomNumber: z.string(),
  GuestEmail: z.string().email(),
  Start: z.string(),
  End: z.string(),
});

type Reservation = z.infer<typeof ReservationSchema>;

export class BookingError extends Error {
  constructor(public readonly messages: string[]) {
    super(messages.join(", "));
  }
}

export async function bookRoom(booking: NewReservation): Promise<Reservation> {
  // unwrap branded types
  const newReservation = {
    ...booking,
    Start: toIsoStr(booking.Start),
    End: toIsoStr(booking.End),
  };

  try {
    return await ky.post("api/reservation", { json: newReservation }).json<Reservation>();
  } catch (error) {
    if (error instanceof HTTPError && error.response.status === 400) {
      const body = await error.response.json<unknown>();
      if (body && typeof body === "object" && "errors" in body) {
        const messages = Object.values(body.errors as Record<string, string[]>).flat();
        throw new BookingError(messages);
      }
      if (typeof body === "string") {
        throw new BookingError([body]);
      }
    }
    throw error;
  }
}

const RoomSchema = z.object({
  number: z.string(),
  state: z.number(),
});

const RoomListSchema = RoomSchema.array();

export function useGetRooms() {
  return useQuery({
    queryKey: ["rooms"],
    queryFn: () => ky.get("api/room").json().then(RoomListSchema.parseAsync),
  });
}
