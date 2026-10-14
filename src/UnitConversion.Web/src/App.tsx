import { useEffect, useState, type FormEvent } from 'react'
import { convert, getUnits, type Unit } from './api/conversions'
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

  const [units, setUnits] = useState<Unit[]>([])
  const [value, setValue] = useState('100')
  const [from, setFrom] = useState('C')
  const [to, setTo] = useState('F')
  const [result, setResult] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getUnits()
      .then((loaded) => setUnits(loaded.filter((u) => u.category === 'temperature')))
      .catch((e: unknown) => setError(e instanceof Error ? e.message : String(e)))
  }, [])

  const handleConvert = async (event: FormEvent) => {
    event.preventDefault()
    setResult(null)
    setError(null)
    try {
      const converted = await convert(Number(value), from, to)
      setResult(`${value} ${from} = ${converted} ${to}`)
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e))
    }
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

      <section className="card">
        <h2>Temperature</h2>
        <form className="convert-form" onSubmit={handleConvert}>
          <input
            type="number"
            step="any"
            required
            aria-label="Value"
            value={value}
            onChange={(e) => setValue(e.target.value)}
          />
          <select aria-label="From unit" value={from} onChange={(e) => setFrom(e.target.value)}>
            {units.map((u) => (
              <option key={u.symbol} value={u.symbol}>
                {u.name}
              </option>
            ))}
          </select>
          <span>→</span>
          <select aria-label="To unit" value={to} onChange={(e) => setTo(e.target.value)}>
            {units.map((u) => (
              <option key={u.symbol} value={u.symbol}>
                {u.name}
              </option>
            ))}
          </select>
          <button type="submit">Convert</button>
        </form>
        {result && <p className="status ok">{result}</p>}
        {error && <p className="status error">{error}</p>}
      </section>
    </main>
  )
}

export default App
