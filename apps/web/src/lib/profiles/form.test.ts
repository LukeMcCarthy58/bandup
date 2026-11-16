import { describe, expect, it } from "vitest";

import {
  formToProfileRequest,
  toFieldErrors,
  validateProfile,
  type ProfileFormValues,
} from "./form";

const valid: ProfileFormValues = {
  displayName: "Jade Morgan",
  bio: "",
  townId: 3,
  level: "Intermediate",
  commitment: "Regular",
  instrumentIds: [6],
  genreIds: [],
};

describe("formToProfileRequest", () => {
  it("reads the form into the API's request shape", () => {
    const form = new FormData();
    form.set("displayName", "  Jade Morgan ");
    form.set("bio", "   ");
    form.set("townId", "3");
    form.set("level", "Intermediate");
    form.set("commitment", "Regular");
    form.append("instrumentIds", "6");
    form.append("instrumentIds", "1");
    form.append("instrumentIds", "nonsense");
    form.append("genreIds", "2");

    expect(formToProfileRequest(form)).toEqual({
      displayName: "Jade Morgan",
      bio: null,
      townId: 3,
      level: "Intermediate",
      commitment: "Regular",
      instrumentIds: [6, 1],
      genreIds: [2],
    });
  });

  it("sends nulls for missing choices so the API can explain what's missing", () => {
    const request = formToProfileRequest(new FormData());
    expect(request.townId).toBeNull();
    expect(request.level).toBeNull();
    expect(request.instrumentIds).toEqual([]);
  });
});

describe("toFieldErrors", () => {
  it("maps PascalCase and camelCase keys to one message per field", () => {
    expect(
      toFieldErrors({ DisplayName: ["Enter your name."], townId: ["Choose your town.", "x"] }),
    ).toEqual({ displayName: "Enter your name.", townId: "Choose your town." });
  });
});

describe("validateProfile", () => {
  it("accepts a complete profile", () => {
    expect(validateProfile(valid)).toEqual({});
  });

  it("explains each missing piece", () => {
    const errors = validateProfile({
      ...valid,
      displayName: " a ",
      townId: null,
      level: null,
      commitment: null,
      instrumentIds: [],
    });
    expect(Object.keys(errors).sort()).toEqual(
      ["commitment", "displayName", "instrumentIds", "level", "townId"].sort(),
    );
  });
});
