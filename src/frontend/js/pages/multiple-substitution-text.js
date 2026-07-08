// Страница конкретной утилиты (выполнение + история)
const MultipleSubstitutionText = {
    async render() {
        const content = document.getElementById('app-content');
        const endpoint = "multi-replace";

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([API.get(`/utilities/${endpoint}`), API.get(`/utilities/${endpoint}/history?limit=10`)]);

            const stars = '★★'.repeat(utility.difficulty) + '☆'.repeat(3 - utility.difficulty);

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
        <label for="text-input">Исходный текст:</label>
        <textarea
            id="text-input"
            rows="6"
            placeholder="Введите текст"></textarea>
    </div>

    <div class="input-group">
        <label>Замены:</label>

        <div id="replacement-list">
            <div class="replacement-row">
                <input type="text" class="replace-from" placeholder="Что заменить">
                <input type="text" class="replace-to" placeholder="На что заменить">
            </div>
        </div>

        <button class="btn" id="btn-add-replacement">
            + Добавить замену
        </button>
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
                            <div><strong>Вход:</strong><code>${UtilityPage.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${UtilityPage.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            document.getElementById("btn-add-replacement").addEventListener("click", () => {
                const list = document.getElementById("replacement-list");

                list.insertAdjacentHTML(
                    "beforeend",
                    `
        <div class="replacement-row">
            <input type="text" class="replace-from" placeholder="Что заменить">
            <input type="text" class="replace-to" placeholder="На что заменить">
        </div>
        `
                );
            });

            // Обработчик кнопки «Выполнить»
            document.getElementById("btn-execute").addEventListener("click", async () => {

                const text = document.getElementById("text-input").value;
                const resultDiv = document.getElementById("exec-result");

                const replacements = {};

                document.querySelectorAll(".replacement-row").forEach(row => {

                    const from = row.querySelector(".replace-from").value.trim();
                    const to = row.querySelector(".replace-to").value;

                    if (from !== "") {
                        replacements[from] = to;
                    }
                });

                const input = JSON.stringify({
                    text,
                    replacements
                });

                resultDiv.innerHTML = '<div class="loading">Выполнение...</div>';

                try {

                    const result = await API.post(
                        `/utilities/${endpoint}/execute`,
                        { input }
                    );

                    if (result.success) {

                        resultDiv.innerHTML = `
                <div class="output-area">
                    <label>Результат:</label>
                    <pre>${MultipleSubstitutionText.escape(result.output)}</pre>
                </div>`;

                    } else {

                        resultDiv.innerHTML =
                            `<div class="error-message">${MultipleSubstitutionText.escape(result.error)}</div>`;
                    }

                } catch (err) {

                    resultDiv.innerHTML =
                        `<div class="error-message">Ошибка: ${err.message}</div>`;
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
