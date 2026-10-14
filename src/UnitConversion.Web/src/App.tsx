import { useEffect, useState } from 'react'
import { getHealth } from './api/health'

type HealthState =
  | { kind: 'checking' }
  | { kind: 'ok'; status: string }
  | { kind: 'error'; message: string }

function App() {
  const [health, setHealth] = useState<HealthState>({ kind: 'checking' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    let ignore = false

    getHealth()
      .then((status) => !ignore && setHealth({ kind: 'ok', status }))
      .catch((error: unknown) => {
        if (!ignore) {
          setHealth({ kind: 'error', message: error instanceof Error ? error.message : String(error) })
        }
      })

    return () => {
      ignore = true
    }
  }, [attempt])

  const checkAgain = () => {
    setHealth({ kind: 'checking' })
    setAttempt((n) => n + 1)
  }

  return (
    <main>
      <h1>Unit Conversion</h1>

      <section className="card">
        <h2>API status</h2>
        {health.kind === 'checking' && <p className="status">Checking…</p>}
        {health.kind === 'ok' && <p className="status ok">API: {health.status}</p>}
        {health.kind === 'error' && (
          <p className="status error">
            API unreachable <span className="detail">({health.message})</span>
          </p>
        )}
        <button type="button" onClick={checkAgain} disabled={health.kind === 'checking'}>
          Check again
        </button>
      </section>
    </main>
  )
}

export default App
