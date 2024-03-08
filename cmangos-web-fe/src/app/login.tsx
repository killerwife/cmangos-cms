import { redirect } from 'next/navigation'
import { useState } from 'react'

const authRequest = (callback: any, username: string, password: string, token: string) => {
    fetch('http://localhost:3080/plain/authorize', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name: username, password: password, token: token }),
    })
        .then((r) => r.json())
        .then((r) => {
            callback(r)
        })
}

export default function Login({ redirectUrl }: {redirectUrl: string}) {
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [token, setToken] = useState('')
    const [usernameError, setUsernameError] = useState('')
    const [passwordError, setPasswordError] = useState('')

    const onButtonClick = () => {
        setUsername('')
        setPasswordError('')

        if ('' === username) {
            setUsernameError('Please enter your username')
            return
        }

        if ('' === password) {
            setPasswordError('Please enter a password')
            return
        }

        authRequest(() => { redirect(redirectUrl) }, username, password, token)
    }

    return (
        <div className={'mainContainer'}>
            <div className={'titleContainer'}>
                <div>Login</div>
            </div>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={username}
                    placeholder="Enter username"
                    onChange={(ev) => setUsername(ev.target.value)}
                    className={'inputBox'}
                />
                <label className="errorLabel">{usernameError}</label>
            </div>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={password}
                    placeholder="Enter password"
                    onChange={(ev) => setPassword(ev.target.value)}
                    className={'inputBox'}
                />
                <label className="errorLabel">{passwordError}</label>
            </div>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={token}
                    placeholder="Enter token if applicable"
                    onChange={(ev) => setToken(ev.target.value)}
                    className={'inputBox'}
                />
            </div>
            <br />
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Log in'} />
            </div>
        </div>
    );
}