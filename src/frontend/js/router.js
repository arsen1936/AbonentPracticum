// Простой хэш-роутер
const Router = {
    _routes: [],

    register(pattern, handler) {
        this._routes.push({
            pattern,
            handler
        });
    },

    navigate(hash) {
        const path = hash.replace('#', '') || 'dashboard';
        for (const {pattern, handler} of this._routes) {
            // Поддержка параметров: utility/:endpoint
            const regex = new RegExp('^' + pattern.replace(/:(\w+)/g, '(?<$1>[^/]+)') + '$');
            const match = path.match(regex);
            if (match) {
                handler(match.groups || {});
                return;
            }
        }
        // 404 — показываем дашборд
        document.getElementById('app-content').innerHTML = `
            <div class="empty-state">
                <h2>Страница не найдена</h2>
                <p><a href="#dashboard">Вернуться на дашборд</a></p>
            </div>`;
    },

    init() {
        window.addEventListener('hashchange', () => this.navigate(location.hash));
        this.navigate(location.hash);
    }
};
