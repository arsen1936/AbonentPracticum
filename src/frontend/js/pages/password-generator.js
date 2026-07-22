// Страница конкретной утилиты (выполнение + история)
const PasswordGeneratorRender = {
    async render(params) {
        const content = document.getElementById('app-content');
        const endpoint = "password-gen";

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
                        <label for="length">Длина пароля:</label>
                        <input
                            type="number"
                            id="length"
                            min="4"
                            max="256"
                            value="16">
                    </div>
                
                    <div class="input-group">
                        <label>
                            <input type="checkbox" id="use-upper" checked>
                            Заглавные буквы (A-Z)
                        </label>
                    </div>
                
                    <div class="input-group">
                        <label>
                            <input type="checkbox" id="use-lower" checked>
                            Строчные буквы (a-z)
                        </label>
                    </div>
                
                    <div class="input-group">
                        <label>
                            <input type="checkbox" id="use-digits" checked>
                            Цифры (0-9)
                        </label>
                    </div>
                
                    <div class="input-group">
                        <label>
                            <input type="checkbox" id="use-symbols" checked>
                            Спецсимволы (!@#$...)
                        </label>
                    </div>
                
                    <button class="btn btn-primary" id="btn-execute">
                        ▶ Сгенерировать
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
                            <div><strong>Вход:</strong><code>${PasswordGeneratorRender.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${PasswordGeneratorRender.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            // Обработчик кнопки «Выполнить»
            document.getElementById("btn-execute").addEventListener("click", async () => {

                const resultDiv = document.getElementById("exec-result");

                resultDiv.innerHTML = '<div class="loading">Генерация...</div>';

                const input = JSON.stringify({
                    length: Number(document.getElementById("length").value),
                    useUpper: document.getElementById("use-upper").checked,
                    useLower: document.getElementById("use-lower").checked,
                    useDigits: document.getElementById("use-digits").checked,
                    useSymbols: document.getElementById("use-symbols").checked
                });

                try {

                    const result = await API.post(
                        `/utilities/${endpoint}/execute`,
                        { input }
                    );

                    if (result.success) {

                        resultDiv.innerHTML = `
                <div class="output-area">
                    <label>Сгенерированный пароль:</label>
                    <pre>${PasswordGeneratorRender.escape(result.output)}</pre>
                </div>`;

                    } else {

                        resultDiv.innerHTML = `
                <div class="error-message">
                    ${PasswordGeneratorRender.escape(result.error)}
                </div>`;
                    }

                } catch (err) {

                    resultDiv.innerHTML = `
            <div class="error-message">
                Ошибка: ${err.message}
            </div>`;
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
