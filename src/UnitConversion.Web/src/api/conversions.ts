export interface Unit {
  symbol: string
  name: string
  category: string
}

/** Returns all units supported by the API. */
export async function getUnits(): Promise<Unit[]> {
  const response = await fetch('/api/units')

  if (!response.ok) {
    throw new Error(`API returned ${response.status} ${response.statusText}`)
  }

  return response.json()
}

/** Asks the API to convert a value; throws with the API's error message on failure. */
export async function convert(value: number, from: string, to: string): Promise<number> {
  const query = new URLSearchParams({ value: String(value), from, to })
  const response = await fetch(`/api/conversions?${query}`)

  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    throw new Error(problem?.detail ?? `API returned ${response.status} ${response.statusText}`)
  }

  const body: { result: number } = await response.json()
  return body.result
}
