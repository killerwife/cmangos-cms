'use client'

import { useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie';

export default function Account() {
    const router = useRouter()
    const [cookies] = useCookies(['access-token'])

    const onChangePassword = () => {
        router.push('/account/changepassword')
    }

    const onAddAuthenticator = () => {

    }

    if (cookies['access-token'] == null) {
        return {
            redirect: {
                destination: '/login',
                permanent: false,
            },
        }
    }

    return (
        <main>
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onChangePassword} value={'Change password'} />
            </div>
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onAddAuthenticator} value={'Add authenticator'} />
            </div>
        </main>
    );
}

