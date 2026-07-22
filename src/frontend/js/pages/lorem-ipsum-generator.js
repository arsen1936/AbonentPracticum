// Страница конкретной утилиты (выполнение + история)
const LoremIpsumGeneratorRender = {
    async render(params) {
        const content = document.getElementById('app-content');
        const endpoint = "lorem-ipsum";

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([
                API.get(`/utilities/${endpoint}`),
                API.get(`/utilities/${endpoint}/history?limit=10`)
            ]);

            const stars = '★'.repeat(utility.difficulty) + '☆'.repeat(3 - utility.difficulty);

            let html = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="utility-detail">
                    <h1>${utility.name}</h1>
                    <div class="meta">
                        <span class="badge badge-category">${utility.category}</span>
                        <span class="difficulty-stars">${stars}</span>
                        ${utility.isImplemented
                ? '<span class="badge badge-ready">Реализована</span>'
                : '<span class="badge badge-todo">Не реализована</span>'}
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
                        <label for="type">Режим генерации:</label>
                        <select id="type">
                            <option value="paragraphs">Абзацы</option>
                            <option value="words">Слова</option>
                            <option value="chars">Символы</option>
                        </select>
                    </div>
                
                    <div class="input-group">
                        <label for="count">Количество:</label>
                        <input
                            type="number"
                            id="count"
                            min="1"
                            value="1"
                            placeholder="Введите количество">
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
                            <div><strong>Вход:</strong><code>${LoremIpsumGeneratorRender.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${LoremIpsumGeneratorRender.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            // Обработчик кнопки «Выполнить»
            document.getElementById("btn-execute").addEventListener("click", async () => {

                const type = document.getElementById("type").value;
                const count = document.getElementById("count").value;

                const resultDiv = document.getElementById("exec-result");
                resultDiv.innerHTML = '<div class="loading">Выполнение...</div>';

                const input = JSON.stringify({
                    type,
                    count
                });

                try {

                    const result = await API.post(
                        `/utilities/${endpoint}/execute`,
                        { input }
                    );

                    if (result.success) {

                        resultDiv.innerHTML = `
                <div class="output-area">
                    <label>Результат:</label>
                    <pre>${UtilityPage.escape(result.output)}</pre>
                </div>`;

                    } else {

                        resultDiv.innerHTML = `
                <div class="error-message">
                    ${UtilityPage.escape(result.error)}
                </div>`;
                    }

                } catch (err) {

                    resultDiv.innerHTML = `
            <div class="error-message">
                Ошибка: ${err.message}
            </div>`;
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
