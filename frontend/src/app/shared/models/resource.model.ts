export type ApiScalar = string | number | boolean | null;
export type ApiResource = Record<string, ApiScalar | ApiScalar[] | ApiResource[]>;
