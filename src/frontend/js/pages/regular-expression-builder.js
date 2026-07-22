// Страница конкретной утилиты (выполнение + история)
const RegularExpressionBuilder = {
    async render() {
        const content = document.getElementById('app-content');
        const endpoint = "regex-tester";

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([API.get(`/utilities/${endpoint}`), API.get(`/utilities/${endpoint}/history?limit=10`)]);

            const stars = '★★★'.repeat(utility.difficulty) + '☆'.repeat(3 - utility.difficulty);

            let html = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="utility-detail">
                    <h1>${utility.name}</h1>
                    <div class="meta">
                        <span class="badge badge-category">${utility.category}</span>
                        <span class="difficulty-stars">${stars}</span>
                        ${utility.isImplemented ? '<span class="badge badge-ready">Реализована</span>' : '<span class="badge badge-todo">Не реализована</span>'}
                    </div>
                    <p>${utility.description}</p>
                </div>`;

            if (!utility.isImplemented) {
                html += `<div class="not-implemented">
                    ⚠️ Эта утилита ещё не реализована. Это задание для практики — реализуйте её в backend (C#) и, при необходимости, добавьте специфичный UI во frontend.
                </div>`;
            }

            html += `
            <div class="utility-detail">
                <div class="input-group">
                    <label for="pattern-input">Регулярное выражение:</label>
                    <input
                        type="text"
                        id="pattern-input"
                        placeholder="Например: \\d{3}-\\d{2}-\\d{2}">
                </div>
            
                <div class="input-group">
                    <label for="flags-input">Флаги:</label>
                    <input
                        type="text"
                        id="flags-input"
                        placeholder="Например: gi">
                </div>
            
                <div class="input-group">
                    <label for="text-input">Текст:</label>
                    <textarea
                        id="text-input"
                        rows="6"
                        placeholder="Введите текст для проверки"></textarea>
                </div>
            
                <button class="btn btn-primary" id="btn-execute">
                    ▶ Выполнить
                </button>
            
                <div id="exec-result"></div>
            </div>`;

            // История
            if (history && history.length > 0) {
                html += `<div class="history-section">
                    <h2>📋 История выполнений</h2>`;
                history.forEach(h => {
                    html += `
                    <div class="history-item">
                        <div class="history-time">${new Date(h.executedAt).toLocaleString('ru-RU')}</div>
                        <div class="history-io">
                            <div><strong>Вход:</strong><code>${RegularExpressionBuilder.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${RegularExpressionBuilder.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            // Обработчик кнопки «Выполнить»
            document.getElementById('btn-execute').addEventListener('click', async () => {
                const pattern = document.getElementById('pattern-input').value;
                const flags = document.getElementById('flags-input').value;
                const text = document.getElementById('text-input').value;
                const resultDiv = document.getElementById('exec-result');

                resultDiv.innerHTML = '<div class="loading">Выполнение...</div>';

                const input = JSON.stringify({
                    pattern,
                    flags,
                    text
                });

                try {
                    const result = await API.post(`/utilities/${endpoint}/execute`, { input });

                    if (result.success) {
                        resultDiv.innerHTML = `
                <div class="output-area">
                    <label>Результат:</label>
                    <pre>${RegularExpressionBuilder.escape(result.output)}</pre>
                </div>`;
                    } else {
                        resultDiv.innerHTML = `
                <div class="error-message">${RegularExpressionBuilder.escape(result.error)}</div>`;
                    }
                } catch (err) {
                    resultDiv.innerHTML = `
            <div class="error-message">Ошибка: ${err.message}</div>`;
                }
            });

            // Ctrl+Enter для выполнения
            document.getElementById('util-input')?.addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.key === 'Enter') {
                    document.getElementById('btn-execute')?.click();
                }
            });

        } catch (err) {
            content.innerHTML = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="error-message">Ошибка загрузки: ${err.message}</div>`;
        }
    },

    escape(str) {
        if (!str) return '';
        str = String(str);
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
};
