import { useState } from 'react';

const API_BASE = 'http://localhost:5000';

export default function App() {
  const [file, setFile] = useState(null);
  const [metadata, setMetadata] = useState({
    documentType: 'manual',
    vehicleModel: '',
    manufacturer: '',
    year: '',
    partNumber: '',
  });
  const [question, setQuestion] = useState('');
  const [status, setStatus] = useState('');
  const [loading, setLoading] = useState(false);
  const [uploadResult, setUploadResult] = useState(null);
  const [answer, setAnswer] = useState('');

  const handleFieldChange = (key, value) => {
    setMetadata((prev) => ({ ...prev, [key]: value }));
  };

  const handleUpload = async (event) => {
    event.preventDefault();

    if (!file) {
      setStatus('PDF を選択してください。');
      return;
    }

    setLoading(true);
    setStatus('PDF を OCR と RAG 用に登録中...');

    try {
      const formData = new FormData();
      formData.append('file', file);

      Object.entries(metadata).forEach(([key, value]) => {
        if (value !== '' && value !== null && value !== undefined) {
          formData.append(key, String(value));
        }
      });

      const response = await fetch(`${API_BASE}/api/pdf/upload`, {
        method: 'POST',
        body: formData,
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data?.title || data?.message || 'PDF upload failed');
      }

      setUploadResult(data);
      setStatus(`アップロード完了: ${file.name}`);
    } catch (error) {
      setStatus(error.message || 'アップロードに失敗しました');
    } finally {
      setLoading(false);
    }
  };

  const handleAsk = async () => {
    if (!question.trim()) {
      setStatus('質問文を入力してください。');
      return;
    }

    setLoading(true);
    setStatus('資料を検索して回答を生成中...');

    try {
      const requestBody = {
        question,
        documentType: metadata.documentType || undefined,
        vehicleModel: metadata.vehicleModel || undefined,
        manufacturer: metadata.manufacturer || undefined,
        year: metadata.year ? Number(metadata.year) : undefined,
        partNumber: metadata.partNumber || undefined,
        limit: 5,
      };

      const response = await fetch(`${API_BASE}/api/chat`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data?.title || '質問に失敗しました');
      }

      const resultText = typeof data.result === 'string' ? data.result : JSON.stringify(data.result, null, 2);
      setAnswer(resultText);
      setStatus('回答を取得しました。');
    } catch (error) {
      setStatus(error.message || '質問に失敗しました');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app-shell">
      <header className="topbar">
        <h1>Vehicle Doc RAG</h1>
      </header>

      <main className="grid">
        <section className="panel">
          <h2>PDF 登録</h2>
          <form onSubmit={handleUpload} className="upload-form">
            <label>
              PDF ファイル
              <input type="file" accept="application/pdf" onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
            </label>

            <label>
              文書種別
              <select value={metadata.documentType} onChange={(e) => handleFieldChange('documentType', e.target.value)}>
                <option value="manual">整備書</option>
                <option value="catalog">部品カタログ</option>
                <option value="wiring">配線図</option>
                <option value="spec">仕様書</option>
              </select>
            </label>

            <label>
              車種
              <input value={metadata.vehicleModel} onChange={(e) => handleFieldChange('vehicleModel', e.target.value)} placeholder="例: GX71" />
            </label>

            <label>
              メーカー
              <input value={metadata.manufacturer} onChange={(e) => handleFieldChange('manufacturer', e.target.value)} placeholder="例: SUZUKI" />
            </label>

            <label>
              年式
              <input type="number" value={metadata.year} onChange={(e) => handleFieldChange('year', e.target.value)} placeholder="2020" />
            </label>

            <label>
              部品番号
              <input value={metadata.partNumber} onChange={(e) => handleFieldChange('partNumber', e.target.value)} placeholder="例: 12345-67890" />
            </label>

            <button type="submit" disabled={loading}>アップロード</button>
          </form>

          {uploadResult && (
            <div className="result-box">
              <strong>アップロード結果</strong>
              <pre>{JSON.stringify(uploadResult, null, 2)}</pre>
            </div>
          )}
        </section>

        <section className="panel chat-panel">
          <h2>質問</h2>
          <textarea
            value={question}
            onChange={(e) => setQuestion(e.target.value)}
            placeholder="例: この車種の交換手順をまとめて"
            rows={6}
          />

          <button onClick={handleAsk} disabled={loading}>質問する</button>

          <div className="status">{status}</div>

          {answer && (
            <div className="result-box answer-box">
              <strong>回答</strong>
              <pre>{answer}</pre>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}
