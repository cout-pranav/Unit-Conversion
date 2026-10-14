/** Calls the API health endpoint and returns its status text (e.g. "Healthy"). */
export async function getHealth(): Promise<string> {
  const response = await fetch('/api/health')

  if (!response.ok) {
    throw new Error(`API returned ${response.status} ${response.statusText}`)
  }

  return response.text()
}
