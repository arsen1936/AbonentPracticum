const DiffCheckerPage = {
    async render() {
        const content = document.getElementById('app-content');
        const endpoint = "text-diff";

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
                </div>
            `;


            if (!utility.isImplemented) {
                html += `
                <div class="not-implemented">
                    ⚠️ Эта утилита ещё не реализована.
                </div>`;
            }


            html += `
            <div class="utility-detail">

                <div class="input-group">
                    <label for="left-input">Первый текст:</label>

                    <textarea
                        id="left-input"
                        rows="10"
                        placeholder="Введите первый текст"></textarea>
                </div>


                <div class="input-group">
                    <label for="right-input">Второй текст:</label>

                    <textarea
                        id="right-input"
                        rows="10"
                        placeholder="Введите второй текст"></textarea>
                </div>


                <button class="btn btn-primary" id="btn-execute">
                    ▶ Сравнить
                </button>


                <div id="exec-result"></div>

            </div>
            `;


            if (history && history.length > 0) {
                html += `
                <div class="history-section">
                    <h2>📋 История выполнений</h2>
                `;

                history.forEach(h => {
                    html += `
                    <div class="history-item">

                        <div class="history-time">
                            ${new Date(h.executedAt).toLocaleString('ru-RU')}
                        </div>

                        <div class="history-io">
                            <div>
                                <strong>Вход:</strong>
                                <code>${DiffCheckerPage.escape(h.input)}</code>
                            </div>

                            <div>
                                <strong>Выход:</strong>
                                <code>${DiffCheckerPage.escape(h.output)}</code>
                            </div>
                        </div>

                    </div>
                    `;
                });

                html += `</div>`;
            }


            content.innerHTML = html;


            document.getElementById('btn-execute')
                .addEventListener('click', async () => {

                    const left =
                        document.getElementById('left-input').value;

                    const right =
                        document.getElementById('right-input').value;

                    const resultDiv =
                        document.getElementById('exec-result');


                    resultDiv.innerHTML =
                        '<div class="loading">Сравнение...</div>';


                    const input = JSON.stringify({
                        left,
                        right
                    });


                    try {
                        const result = await API.post(
                            `/utilities/${endpoint}/execute`,
                            { input }
                        );


                        if (result.success) {

                            resultDiv.innerHTML = `
                            <div class="output-area">

                                <label>Diff:</label>

                                <pre>${DiffCheckerPage.escape(result.output)}</pre>

                            </div>
                            `;
                        }
                        else {
                            resultDiv.innerHTML = `
                            <div class="error-message">
                                ${DiffCheckerPage.escape(result.error)}
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
            <a href="#dashboard" class="back-link">
                ← Назад к списку
            </a>

            <div class="error-message">
                Ошибка загрузки: ${err.message}
            </div>`;
        }
    },


    escape(str) {
        if (!str)
            return '';

        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
};