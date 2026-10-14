import { useEffect, useState, type FormEvent } from 'react'
import { convert, getUnits, type Unit } from './api/conversions'

interface Conversion {
  value: number
  from: Unit
  to: Unit
  result: number
}

const capitalize = (s: string) => s.charAt(0).toUpperCase() + s.slice(1)

function App() {
  const [allUnits, setAllUnits] = useState<Unit[]>([])
  const [category, setCategory] = useState('temperature')
  const [value, setValue] = useState('100')
  const [from, setFrom] = useState('C')
  const [to, setTo] = useState('F')
  const [result, setResult] = useState<Conversion | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [converting, setConverting] = useState(false)

  useEffect(() => {
    getUnits()
      .then(setAllUnits)
      .catch((e: unknown) => setError(`Couldn't load units from the API: ${e instanceof Error ? e.message : String(e)}`))
  }, [])

  const categories = [...new Set(allUnits.map((u) => u.category))]
  const units = allUnits.filter((u) => u.category === category)
  const findUnit = (symbol: string) => units.find((u) => u.symbol === symbol)

  const clearOutput = () => {
    setResult(null)
    setError(null)
  }

  const changeCategory = (next: string) => {
    const nextUnits = allUnits.filter((u) => u.category === next)
    setCategory(next)
    setFrom(nextUnits[0]?.symbol ?? '')
    setTo(nextUnits[1]?.symbol ?? nextUnits[0]?.symbol ?? '')
    clearOutput()
  }

  const swapUnits = () => {
    setFrom(to)
    setTo(from)
    clearOutput()
  }

  const handleConvert = async (event: FormEvent) => {
    event.preventDefault()
    clearOutput()
    setConverting(true)
    try {
      const converted = await convert(Number(value), from, to)
      const fromUnit = findUnit(from)
      const toUnit = findUnit(to)
      if (fromUnit && toUnit) {
        setResult({ value: Number(value), from: fromUnit, to: toUnit, result: converted })
      }
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e))
    } finally {
      setConverting(false)
    }
  }

  const unitOptions = units.map((u) => (
    <option key={u.symbol} value={u.symbol}>
      {u.name} ({u.symbol})
    </option>
  ))

  return (
    <main className="page">
      <header className="masthead">
        <h1>Unit Conversion</h1>
        <p>Convert temperature, length and mass.</p>
      </header>

      <section className="panel" aria-label="Converter">
        <div className="segmented" role="group" aria-label="Category">
          {categories.map((c) => (
            <button
              key={c}
              type="button"
              aria-pressed={c === category}
              onClick={() => changeCategory(c)}
            >
              {capitalize(c)}
            </button>
          ))}
        </div>

        <form className="converter" onSubmit={handleConvert}>
          <label className="field field-value">
            <span>Value</span>
            <input
              type="number"
              step="any"
              required
              inputMode="decimal"
              value={value}
              onChange={(e) => setValue(e.target.value)}
            />
          </label>

          <label className="field field-from">
            <span>From</span>
            <select value={from} onChange={(e) => setFrom(e.target.value)}>
              {unitOptions}
            </select>
          </label>

          <button type="button" className="swap" onClick={swapUnits} aria-label="Swap units" title="Swap units">
            ⇄
          </button>

          <label className="field field-to">
            <span>To</span>
            <select value={to} onChange={(e) => setTo(e.target.value)}>
              {unitOptions}
            </select>
          </label>

          <button type="submit" className="primary" disabled={converting || units.length === 0}>
            {converting ? 'Converting…' : 'Convert'}
          </button>
        </form>

        <output className={`readout${error ? ' readout-error' : ''}`} aria-live="polite">
          {error ? (
            <p className="readout-message">{error}</p>
          ) : result ? (
            <>
              <p className="readout-number">
                {result.result.toLocaleString(undefined, { maximumFractionDigits: 5 })}
                <span className="readout-unit">{result.to.symbol}</span>
              </p>
              <p className="readout-detail">
                {result.value.toLocaleString(undefined, { maximumFractionDigits: 5 })} {result.from.name} ={' '}
                {result.result.toLocaleString(undefined, { maximumFractionDigits: 5 })} {result.to.name}
              </p>
            </>
          ) : (
            <p className="readout-message">
              {allUnits.length === 0 ? 'Loading units…' : 'Enter a value, pick the units, then convert.'}
            </p>
          )}
        </output>
      </section>
    </main>
  )
}

export default App
