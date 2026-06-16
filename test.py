import os


def scan_unity_project():
    assets_path = os.path.join(".", "Assets")
    if not os.path.exists(assets_path):
        print("❌ Ошибка: Запустите скрипт в корневой папке Unity-проекта (где лежит Assets).")
        return

    print("--- START OF STRUCTURE ---")
    for root, dirs, files in os.walk(assets_path):
        # Игнорируем скрытые папки и плагины, если нужно, но смотрим на .cs
        cs_files = [ f for f in files if f.endswith('.cs') ]
        if cs_files:
            relative_path = os.path.relpath(root, assets_path)
            indent = "  " * (relative_path.count(os.sep) + 1)
            print(f"📂 Assets/{relative_path.replace(os.sep, '/')}")
            for f in cs_files:
                print(f"{indent}📄 {f}")
    print("--- END OF STRUCTURE ---")


if __name__ == "__main__":
    scan_unity_project()